using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows;
using System.Text.RegularExpressions;
using System.Globalization;

namespace mongoDBDemo
{

    class TrafikverketProcessor
    {
        public async Task<TrafikverketModel> LoadTrafikVerket()
        {
            string url = "https://api.trafikinfo.trafikverket.se/v2/data.json";


        using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    TrafikverketModel trafikverket = await response.Content.ReadAsAsync<TrafikverketModel>();

                    return trafikverket;
                }
                else
                {
                    
                    throw new Exception(response.ReasonPhrase);
                    
                }
            }
     
        }

        public static async void PostTrafikVerket(HttpContent data)

        {
            string url = "https://api.trafikinfo.trafikverket.se/v2/data.json";
            using (HttpResponseMessage response = await ApiHelper.ApiClient.PostAsync(url, data))
            {
                if (response.IsSuccessStatusCode)
                {
                    
                    string fromTrafikverket = await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(fromTrafikverket);


                    // json objectet från trafikverket har strukturen ("RESPONSE") ("RESULT") [ ("TrainStation") ]
                    
                    var trainStations = jsonObject.SelectToken("RESPONSE").SelectToken("RESULT")[0].SelectToken("TrainStation");
                    
                    MongoCRUD db = new MongoCRUD("admin");
                    foreach (var z in trainStations)
                    {
                        TrafikverketModel model = new TrafikverketModel();

                        model.AdvertisedLocationName = z[key: "AdvertisedLocationName"].ToString();
                        model.LocationSignature = z[key: "LocationSignature"].ToString();

                        // Fortsättning på kod, läs upp från databasen för att kolla efter dubbleter i DB 

                        db.InsertRecord("Station", model);

                    }

                }
                else
                {
                    Console.WriteLine("!!FEL VID KONTAKT MED API!!");
                    throw new Exception(response.ReasonPhrase);
                }

            }

        }

        public static async void PostTrainMessage(HttpContent data)

        {
            string url = "https://api.trafikinfo.trafikverket.se/v2/data.json";
            using (HttpResponseMessage response = await ApiHelper.ApiClient.PostAsync(url, data))
            {
                if (response.IsSuccessStatusCode)
                {

                    string fromTrafikverket = await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(fromTrafikverket);

                    
                    // json objectet från trafikverket har strukturen ("RESPONSE") ("RESULT") [ ("TrainStation") ]

                    var trainMessage = jsonObject.SelectToken("RESPONSE").SelectToken("RESULT")[0].SelectToken("TrainMessage");
                    //Console.WriteLine(trainMessage.ToString());
                    MongoCRUD db = new MongoCRUD("admin");
                    foreach (var message in trainMessage)
                    {
                        TrafikverketModelTrainMessage model = new TrafikverketModelTrainMessage();

                        // sätter in alla värden enligt modellen i TrafikverketModel
                        model.EventId = message[key: "EventId"].ToString();
                        model.Header = message[key: "Header"].ToString();
                        model.ExternalDescription = message[key: "ExternalDescription"].ToString();

                        JToken geo = message[key: "Geometry"];
                        JToken wgs84 = geo[key: "WGS84"];
                        string pointstring = wgs84.ToString();


                        // input.replace(/^\((.+)\)$/,"[$1]");
                        pointstring = Regex.Replace(pointstring, "POINT", "");

                        pointstring = Regex.Replace(pointstring, @"[()]", "");
                        //pointstring = Regex.Replace(pointstring, @"([()])", "");

                        string[] substring;
                        Point geometric;

                        substring = Regex.Split(pointstring, " ");

                        string[] coords = { substring[1], substring[2] };
                        //string[] coords = pointstring.Split(' ');

                        PointF point = new PointF(float.Parse(coords[0], CultureInfo.InvariantCulture), float.Parse(coords[1], CultureInfo.InvariantCulture));

                        model.GeometryWGS84 = point;

                        Console.WriteLine(point.GetType);
                        //JToken x = message[key: "GeometryWGS84"];
                        

                        //Console.WriteLine(message[key: "GeometryWGS84"].GetType());
                        

                        model.StartDateTime = message[key: "StartDateTime"].ToObject<DateTime>();
                        //model.EndDateTime = message[key: "EndDateTime"].ToObject<DateTime>();
                        model.ModifiedTime = message[key: "ModifiedTime"].ToString();
                        model.AffectedLocation = message[key: "AffectedLocation"].ToObject<List<string>>();

                        db.InsertRecord("TrainMessage", model);

                    }
                    
                }
                else
                {
                    Console.WriteLine("!!FEL VID KONTAKT MED API!!");
                    throw new Exception(response.ReasonPhrase);
                }

            }

        }
    }

}
