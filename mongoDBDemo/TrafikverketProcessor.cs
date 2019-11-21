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

        public static async Task<TrafikverketModel> PostTrafikVerket(HttpContent data)
        {
            string url = "https://api.trafikinfo.trafikverket.se/v2/data.json";
            using (HttpResponseMessage response = await ApiHelper.ApiClient.PostAsync(url, data))
            {
                if (response.IsSuccessStatusCode)
                {
                    // fel verkar uppstå när datan ska hämtas som modell
                    TrafikverketModel trafikverket = await response.Content.ReadAsAsync<TrafikverketModel>();
                    Console.WriteLine("kontakt OK!");

                    // Detta test visar om data hämtas
                    //string test = await response.Content.ReadAsStringAsync();
                    //Console.WriteLine(test);


                    return trafikverket;
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
