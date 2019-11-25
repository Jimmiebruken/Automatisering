using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace mongoDBDemo
{
    class VastTrafik
    {
        public static async Task<VastTrafikModel> LoadVastTrafik()
        {
            string url = "https://api.vasttrafik.se/ts/v1/traffic-situations";


            using (HttpResponseMessage response = await ApiHelper.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    VastTrafikModel vastTrafik = await response.Content.ReadAsAsync<VastTrafikModel>();

                    return vastTrafik;
                }
                else
                {

                    throw new Exception(response.ReasonPhrase);

                }
            }

        }






    }
}
