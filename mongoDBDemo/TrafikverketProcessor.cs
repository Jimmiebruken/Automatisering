using MongoDB.Bson;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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
    }
}
