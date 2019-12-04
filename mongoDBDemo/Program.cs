using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace mongoDBDemo
{
    class Program
    {
        //test
        static async Task Main(string[] args)
        {
            MongoCRUD db = new MongoCRUD("admin");

            ApiHelper.InitializeClient();
            //ApiHelper.GetToken();
            //startar en webclient
            //ApiHelper.InitializeClientVastTrafik();


            TrafikverketProcessor.LoopTrainAnnouncement();
            //TrafikverketProcessor.PostTrainAnnouncement(Query.TrainAnnouncement("F"));

            //Kalla på metoden postrafikverket
           // TrafikverketProcessor.PostTrainStation(Query.TrainStation());

            //VastTrafikProcessor.GetLocationName();
            
           // var testlist = db.FindRecord<string>("Locationsss", "Name", "Västra Parken");
           // Console.WriteLine(testlist);
           
      
            // Ligger endast för att blocka körningen från avslut ( för att kolla fel/ok medelanden)
            Console.ReadLine();

        }
    }

    
    



}
