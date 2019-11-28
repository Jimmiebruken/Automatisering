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

                    Console.WriteLine(trafficSituation[0]);

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
                        //model.affectedStopPoints = traffic[key: "affectedStopPoints[0]"].ToObject<List<string>>();

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
    }
}
