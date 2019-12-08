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
       
        
        public static void LoopTrainAnnouncement()
        {
            IList<string> signList = new List<string>();

            signList = MongoCRUD.FindStationSign("14");
            foreach (var sign in signList)
            {
                TrafikverketProcessor.PostTrainAnnouncement(Query.TrainAnnouncement(sign));
            }
            
        }

        public static async void PostTrainAnnouncement(HttpContent data)
        {
            string url = "https://api.trafikinfo.trafikverket.se/v2/data.json";


            using (HttpResponseMessage response = await ApiHelper.ApiClient.PostAsync(url, data))
            {
                if (response.IsSuccessStatusCode)
                {
                    string fromTrafikverket = await response.Content.ReadAsStringAsync();
                    JObject jsonObject = JObject.Parse(fromTrafikverket);

                    var trainAnnouncements = jsonObject.SelectToken("RESPONSE").SelectToken("RESULT")[0].SelectToken("TrainAnnouncement");
                    MongoCRUD db = new MongoCRUD("admin");

                    foreach( var announcement in trainAnnouncements)
                    {
                        TrafikverketTrainAnnouncementModel model = new TrafikverketTrainAnnouncementModel();

                        try
                        {
                            try { model.ActivityId = announcement[key: "ActivityId"].ToString(); } catch { }

                            try { model.AdvertisedTimeAtLocation = announcement[key: "AdvertisedTimeAtLocation"].ToObject<DateTime>(); } catch { }
                            try { model.EstimatedTimeAtLocation = announcement[key: "EstimatedTimeAtLocation"].ToObject<DateTime>(); }
                            // för många poster utan estimatedTimeAtLocation, skippar loggning av fel
                            catch { }
                            try { model.Canceled = announcement[key: "Canceled"].ToObject<bool>(); } catch { }
                            try { model.InformationOwner = announcement[key: "InformationOwner"].ToString(); } catch {}
                            try { model.LocationSignature = announcement[key: "LocationSignature"].ToString(); } catch {}
                            
                            
                            try 
                            {
                                var token = announcement[key: "FromLocation"][0];
                                string stringFromLocation = token.First.ToString();
                                stringFromLocation = Regex.Replace(stringFromLocation, "\"", "");
                                string[] substring = Regex.Split(stringFromLocation, " ");
                                model.FromLocation = substring[1];
                            } catch {  }

                            try
                            {
                                var token = announcement[key: "ToLocation"][0];
                                string stringToLocation = token.First.ToString();
                                stringToLocation = Regex.Replace(stringToLocation, "\"", "");
                                string[] substring = Regex.Split(stringToLocation, " ");
                                model.ToLocation = substring[1];
                            }
                            catch
                            {
                               
                            }

                            try
                            {
                                model.Deviation = announcement[key: "Deviation"][0].ToString();
                                
                            }
                            catch
                            {

                            }

                            await db.Upsert(Trafikverket.trainAnnouncement, model, model.ActivityId);
                            //db.InsertRecord("TrainAnnouncement", model);
                        }
                        catch
                        {
                            Console.WriteLine("error reading/writing");
                        }
                        
                    }




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
                        TrafikverketTrainStationModel model = new TrafikverketTrainStationModel();

                        
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

                        

                        db.InsertRecord(Trafikverket.station, model);
                       

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
                        TrafikverketTrainMessageModel model = new TrafikverketTrainMessageModel();

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
                            //Console.WriteLine("No endate time set for model");
                        }

                        //db.InsertRecord("TrainMessage", model);
                        await db.Upsert(Trafikverket.trainMessage, model, model.EventId);
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
