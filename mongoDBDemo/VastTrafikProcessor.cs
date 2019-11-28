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


                    JArray i = JArray.Parse(fromVastTrafik);

                    Console.WriteLine(i[0].Type);

                    MongoCRUD db = new MongoCRUD("admin");

                    foreach (var sit in i)
                    {
                        VastTrafikModelTrafficSituation model = new VastTrafikModelTrafficSituation();

                        model.situationNumber = sit[key: "situationNumber"].ToString();
                        model.title = sit[key: "title"].ToString();

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
