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
            using (HttpResponseMessage response = await ApiHelperVasttrafik.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    string fromVastTrafik = await response.Content.ReadAsStringAsync();

                    JArray trafficSituation = JArray.Parse(fromVastTrafik);

                    MongoCRUD db = new MongoCRUD("admin");

                    foreach (var traffic in trafficSituation)
                    {
                        VastTrafikModelTrafficSituation model = new VastTrafikModelTrafficSituation();

                        model.SituationNumber = traffic[key: "situationNumber"].ToString();
                        model.CreationTime = traffic[key: "creationTime"].ToObject<DateTime>();
                        model.StartTime = traffic[key: "startTime"].ToObject<DateTime>();
                        model.EndTime = traffic[key: "endTime"].ToObject<DateTime>();
                        model.Severity = traffic[key: "severity"].ToString();
                        model.Title = traffic[key: "title"].ToString();
                        model.Description = traffic[key: "description"].ToString();
                        model.AffectedStopPoints = new List<AffectedStopPointsModel>();

                        

                        try
                        {
                            JArray affectedStopPoints = JArray.Parse(traffic.SelectToken("affectedStopPoints").ToString());

                            foreach (var stops in affectedStopPoints)
                            {
                                AffectedStopPointsModel modelaffected = new AffectedStopPointsModel();
                                modelaffected.Name = stops[key: "name"].ToString();

                                modelaffected.StopPointGid = stops[key: "gid"].ToString();
                                modelaffected.MunicipalityName = stops[key: "municipalityName"].ToString();

                                model.AffectedStopPoints.Add(modelaffected);

                                StopPointNameMunicipalityModel stopName = new StopPointNameMunicipalityModel();
                                stopName.Name = stops[key: "name"].ToString();
                                stopName.MunicipalityName = stops[key: "municipalityName"].ToString();
                                stopName.SituationNumber = model.SituationNumber.ToString();

                                try
                                {
                                    JArray affectedLines = JArray.Parse(traffic.SelectToken("affectedLines").ToString());
                                    

                                    foreach (var stop in affectedLines)
                                    {

                                        
                                        stopName.TransportAuthorityName = stop[key: "transportAuthorityName"].ToString();
                                        stopName.DefaultTransportModeCode = stop[key: "defaultTransportModeCode"].ToString();
                                        

                                        
                                    }
                                }
                                catch
                                {
                                    Console.WriteLine("affectedLines Error");
                                    break;
                                }


                                GetGeo(stopName, stopName.Name, stopName.MunicipalityName);
                                await db.Upsert(Vasttrafik.affectedLocation, stopName);
                            }
                        }
                        catch
                        {
                            Console.WriteLine("Catchen");
                            break;
                        }


                        
                        /*
                        try
                        {
                            JArray affectedLines = JArray.Parse(traffic.SelectToken("affectedLines").ToString());
                            Console.WriteLine(affectedLines);

                            foreach (var stop in affectedLines)
                            {

                                StopPointNameMunicipalityModel modelaffected = new StopPointNameMunicipalityModel();
                                modelaffected.TransportAuthorityName = stop[key: "transportAuthorityName"].ToString();
                                modelaffected.DefaultTransportModeCode = stop[key: "defaultTransportModeCode"].ToString();
                                modelaffected.SituationNumber = model.SituationNumber.ToString();

                                await db.Upsert(Vasttrafik.affectedLocation, modelaffected);
                            }
                        }
                        catch
                        {
                            Console.WriteLine("affectedLines Error");
                            break;
                        }
                        */
    



                        await db.Upsert(Vasttrafik.traficSituation, model);
                        
                    }
                }
                else
                {
                    Console.Write("Fel vid kontakt med API");
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
        public static async void GetLocationName(string street, string municipality)
        {

            string url = "https://api.vasttrafik.se/bin/rest.exe/v2/location.name?input="+street + "%20" + municipality + "&format=json";
            //string url = "https://api.vasttrafik.se/bin/rest.exe/v2/location.name?input=sanneg%C3%A5rdshamnen&format=json";
            using (HttpResponseMessage response = await ApiHelperVasttrafik.ApiClient.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    string fromVastTrafik = await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(fromVastTrafik);

                    var locationName = jsonObject.SelectToken("LocationList").SelectToken("StopLocation")[0];
                    
                    MongoCRUD db = new MongoCRUD("admin");

                        VastTrafikModelLocation model = new VastTrafikModelLocation();

                        model.Name = locationName[key: "name"].ToString();
                        model.Lon = locationName[key: "lon"].ToObject<float>();
                        model.Lat = locationName[key: "lat"].ToObject<float>();




                    db.InsertRecord(Vasttrafik.locations, model);
                }
                else
                {
                    Console.Write("Fel vid kontakt med API");
                    throw new Exception(response.ReasonPhrase);
                }

            }
        }



        public static async void GetGeo(StopPointNameMunicipalityModel model, string street, string municipality)
        {
            try
            {
                string url = "https://api.vasttrafik.se/bin/rest.exe/v2/location.name?input=" + street + "%20" + municipality + "&format=json";
                //string url = "https://api.vasttrafik.se/bin/rest.exe/v2/location.name?input=sanneg%C3%A5rdshamnen&format=json";
                using (HttpResponseMessage response = await ApiHelperVasttrafik.ApiClient.GetAsync(url))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string fromVastTrafik = await response.Content.ReadAsStringAsync();
                        JObject jsonObject = JObject.Parse(fromVastTrafik);

                        var locationName = jsonObject.SelectToken("LocationList").SelectToken("StopLocation")[0];
                        //Console.WriteLine(model);

                        MongoCRUD db = new MongoCRUD("admin");



                        model.Lon = locationName[key: "lon"].ToObject<float>();
                        model.Lat = locationName[key: "lat"].ToObject<float>();

                        //Tuple coordReturn = new Tuple(longitude, latitude);




                        await db.Upsert(Vasttrafik.affectedLocation, model);
                    }
                    else
                    {
                        Console.Write("Fel vid kontakt med Västtrafik API");
                        Console.WriteLine(response.ReasonPhrase);
                        //throw new Exception(response.ReasonPhrase);
                    }

                }
            }
            catch { }
        }
    }
}
