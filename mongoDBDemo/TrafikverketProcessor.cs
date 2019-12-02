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

        public static async void PostTrainStation(HttpContent data)

        {
            string url = "https://api.trafikinfo.trafikverket.se/v2/data.json";

            int errorCountyNo = 0;
            using (HttpResponseMessage response = await ApiHelper.ApiClient.PostAsync(url, data))

            {
                if (response.IsSuccessStatusCode)
                {

                    string fromTrafikverket = await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(fromTrafikverket);


                    // json objectet från trafikverket har strukturen ("RESPONSE") ("RESULT") [ ("TrainStation") ]

                    var trainStations = jsonObject.SelectToken("RESPONSE").SelectToken("RESULT")[0].SelectToken("TrainStation");

                    MongoCRUD db = new MongoCRUD("admin");
                    foreach (var message in trainStations)
                    {
                        TrafikverketModel model = new TrafikverketModel();

                        
                        model.AdvertisedLocationName = message[key: "AdvertisedLocationName"].ToString();
                        model.LocationSignature = message[key: "LocationSignature"].ToString();

                        try
                        {

                            model.CountyNo = message[key: "CountyNo"][0].ToString();
                        }
                        catch
                        {
                            errorCountyNo += 1;
                        }
                        JToken geo = message[key: "Geometry"];
                        JToken wgs84 = geo[key: "WGS84"];
                        string pointstring = wgs84.ToString();


                        // Regex för att ta bort "POINT" från fältet och sedan "( )"
                        pointstring = Regex.Replace(pointstring, "POINT", "");
                        pointstring = Regex.Replace(pointstring, @"[()]", "");

                        // delar stringen på alla mellanrum. !OBS fältet startar med mellanrum vilket skapar 3! delar
                        string[] substring = Regex.Split(pointstring, " ");

                        // plockar upp och delar ut cordinaterna, mest för läsbarhet och att inte råka skicka med mellanrummet
                        string[] coords = { substring[1], substring[2] };
                        model.Longitude = float.Parse(coords[0], CultureInfo.InvariantCulture);
                        model.Latitude = float.Parse(coords[1], CultureInfo.InvariantCulture);




                        // Fortsättning på kod, läs upp från databasen för att kolla efter dubbleter i DB 

                        

                        db.InsertRecord("Station", model);

                    }

                    Console.WriteLine("Failed to get " + errorCountyNo + " number of county numbers from API");

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
                        //model.EventId = message[key: "EventId"].ToString();
                        //model.Header = message[key: "Header"].ToString();
                        model.ExternalDescription = message[key: "ExternalDescription"].ToString();

                        JToken geo = message[key: "Geometry"];
                        JToken wgs84 = geo[key: "WGS84"];
                        string pointstring = wgs84.ToString();


                        // Regex för att ta bort "POINT" från fältet och sedan "( )"
                        pointstring = Regex.Replace(pointstring, "POINT", "");
                        pointstring = Regex.Replace(pointstring, @"[()]", "");

                        // delar stringen på alla mellanrum. !OBS fältet startar med mellanrum vilket skapar 3! delar
                        string[] substring = Regex.Split(pointstring, " ");

                        // plockar upp och delar ut cordinaterna, mest för läsbarhet och att inte råka skicka med mellanrummet
                        string[] coords = { substring[1], substring[2] };
                        model.Longitude = float.Parse(coords[0], CultureInfo.InvariantCulture);
                        model.Latitude = float.Parse(coords[1], CultureInfo.InvariantCulture);


                        model.StartDateTime = message[key: "StartDateTime"].ToObject<DateTime>();
                        model.ModifiedTime = message[key: "ModifiedTime"].ToString();
                        model.AffectedLocation = message[key: "AffectedLocation"].ToObject<List<string>>();


                        // försöker lägga till EndDate om detta finns annars skriver ut felmedelande i consolen
                        try
                        {
                            model.EndDateTime = message[key: "EndDateTime"].ToObject<DateTime>();
                        }
                        
                        catch
                        {
                            Console.WriteLine("No endate time set for model");
                        }

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
