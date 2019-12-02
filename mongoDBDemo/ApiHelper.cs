using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace mongoDBDemo
{
    public static class  ApiHelper
    {
        public static HttpClient ApiClient { get; set; } 

        public static void InitializeClient()
        {
            
            ApiClient = new HttpClient();
            ApiClient.DefaultRequestHeaders.Accept.Clear();

            // krash om denna används
            //ApiClient.DefaultRequestHeaders.Add("content-type", "application/json");



        }

        public static void InitializeClientVastTrafik()
        {

            ApiClient = new HttpClient();
            ApiClient.DefaultRequestHeaders.Accept.Clear();

            ApiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "65994c74-63c6-3f61-acf1-492f24d37702");

        }

    }
}
