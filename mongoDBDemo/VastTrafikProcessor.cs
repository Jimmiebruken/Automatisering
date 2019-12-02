using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace mongoDBDemo
{
    class VastTrafikProcessor
    {
        public static async void GetTrafficSituation()
        {
            string url = "https://api.vasttrafik.se/ts/v1/traffic-situations";
            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))

            {
                if (response.IsSuccessStatusCode)
                {
                    string fromVastTrafik = await response.Content.ReadAsStringAsync();

                    JArray trafficSituation = JArray.Parse(fromVastTrafik);

                    //Console.WriteLine(trafficSituation[0].Type);

                    MongoCRUD db = new MongoCRUD("admin");

                    foreach (var traffic in trafficSituation)
                    {
                        VastTrafikModelTrafficSituation model = new VastTrafikModelTrafficSituation();

                        model.situationNumber = traffic[key: "situationNumber"].ToString();
                        model.creationTime = traffic[key: "creationTime"].ToObject<DateTime>();
                        model.startTime = traffic[key: "startTime"].ToObject<DateTime>();
                        model.endTime = traffic[key: "endTime"].ToObject<DateTime>();
                        model.severity = traffic[key: "severity"].ToString();
                        model.title = traffic[key: "title"].ToString();
                        model.description = traffic[key: "description"].ToString();
                        // try
                        //{
                        //  var stopPoints = traffic[key: "affectedStopPoints"].ToObject<List<string>>();
                        //Console.WriteLine(traffic[key: "affectedStopPoints"].ToObject<List<string>>());

                        // foreach (var i in stopPoints)
                        //{

                        //  model.affectedStopPoints = i.ToObject<List<string>>();
                        //Console.WriteLine(i);
                        //}
                        // }
                        //catch
                        //{
                        //  break;
                        // }

                        //model.affectedStopPoints = traffic[key: "affectedStopPoints"][0].ToObject<List<string>>();

                       
                        db.InsertRecord("Traffic-Situations", model);
                    }
                }
                else
                {
                    Console.Write("Fel vid kontakt med API");
                    throw new Exception(response.ReasonPhrase);

                }
            }

        }
        public static async void GetLocationName()
        {
            string url = "https://api.vasttrafik.se/bin/rest.exe/v2/location.name?input=sanneg%C3%A5rdshamnen&format=json";
            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    string fromVastTrafik = await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(fromVastTrafik);

                    var locationName = jsonObject.SelectToken("LocationList").SelectToken("StopLocation")[0];
                    Console.WriteLine(locationName);


                    MongoCRUD db = new MongoCRUD("admin");

                   // foreach (var location in locationName)
                    //{
                        VastTrafikModelLocation model = new VastTrafikModelLocation();

                        model.name = locationName[key: "name"].ToString();
                        model.lon = locationName[key: "lon"].ToObject<float>();
                        model.lat = locationName[key: "lat"].ToObject<float>();
                        Console.WriteLine(locationName[key: "name"]);
                        Console.WriteLine(locationName[key: "lon"]);
                        Console.WriteLine(locationName[key: "lat"]);

                    db.InsertRecord("Locations_test", model);
                   // }
                }
                else
                {
                    Console.Write("Fel vid kontakt med API");
                    throw new Exception(response.ReasonPhrase);
                }

            }
        }
    }
}
