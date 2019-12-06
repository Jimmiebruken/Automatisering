using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Timers;

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


            
            //TrafikverketProcessor.PostTrainAnnouncement(Query.TrainAnnouncement("F"));

            //Kalla på metoden postrafikverket OBS! för att hämta tågstationer krävs att _id i TrafikverketModel.cs är borttaget men detta krockar med TrainAnnouncement senare
            //TrafikverketProcessor.PostTrainStation(Query.TrainStation());

            //VastTrafikProcessor.GetLocationName();

            // var testlist = db.FindRecord<string>("Locationsss", "Name", "Västra Parken");
            // Console.WriteLine(testlist);

            // skapar en timer för programmet, inställningar anger hur ofta metoderna ska loopas igenom
            Timer timer = new Timer();

            // endast timerMinutes behöver ändras för att sätta intervallerna på timern
            int timerMinutes = 1;
            timer.Interval = 1000 * 60 * timerMinutes;
            timer.Elapsed += OnTimedEvent;
            timer.Enabled = true;



            // Ligger endast för att blocka körningen från avslut ( för att kolla fel/ok medelanden)
            Console.WriteLine("Script has started - press any key to exit");
            Console.ReadLine();

        }


        // funktionen blir kallad varje gång timern har räknat ner, kalla endast på huvudfunktionerna här
        public static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
        {
            TrafikverketProcessor.LoopTrainAnnouncement();
            TrafikverketProcessor.PostTrainMessage(Query.TrainMessage());
            Console.WriteLine("one batch completed, waiting for next batch");
        }
    }

    
    



}
