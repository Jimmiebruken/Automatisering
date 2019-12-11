using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Collections.Specialized;
using RestSharp;

namespace mongoDBDemo
{
    public static class ApiHelper
    {
        public static HttpClient ApiClient { get; set; }

        public static void InitializeClient()
        {
            ApiClient = new HttpClient();
            ApiClient.DefaultRequestHeaders.Accept.Clear();
        }

      
    }

    public static class ApiHelperVasttrafik
    {
        public static HttpClient ApiClient { get; set; }

        public static void InitializeClient()
        {
            ApiClient = new HttpClient();
            ApiClient.DefaultRequestHeaders.Accept.Clear();
        }

        public static void GetToken()
        {
            
            var client = new RestClient("https://api.vasttrafik.se/token");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Authorization", "basic UXU0Y1NVb2JOR3hGNWlwSDFCUnRmNzJMUFlNYTp0MWxXWEVCVEl0ZlNiZmFHTE1qaXA2UmZXU01h");
            request.AddParameter("application/json", "grant_type=client_credentials&scope=device_12345", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            string test = response.Content;
            JObject jsonObject = JObject.Parse(test);

            string token = jsonObject.SelectToken("access_token").ToString();

            ApiClient = new HttpClient();
            ApiClient.DefaultRequestHeaders.Accept.Clear();

            ApiClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
        }
    }
}
