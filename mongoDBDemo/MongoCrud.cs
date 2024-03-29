﻿using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace mongoDBDemo
{

    // MongoCRUD sköter kontakten med DB, OBS! DB IP är satt till LAN! Public MONGODB IP är 78.67.178.206
    public class MongoCRUD
    {
        public static IMongoDatabase db;

        public  MongoCRUD(string database)
        {
            var client = new MongoClient("mongodb://192.168.1.66:27017/admin");
            db = client.GetDatabase(database);

        }

        public void InsertRecord<T>(string table, T record)
        {
            var collection = db.GetCollection<T>(table);
            collection.InsertOne(record);
        }
        public async Task Upsert(string  table, TrafikverketTrainAnnouncementModel record, string ActivityId)
        {
            var collection = db.GetCollection<TrafikverketTrainAnnouncementModel>(table);


            collection.ReplaceOne(filter: (u => u.ActivityId  == ActivityId),options: new UpdateOptions { IsUpsert = true }, replacement: record);
        }
        public async Task Upsert(string table, TrafikverketTrainMessageModel record)
        {
            var collection = db.GetCollection<TrafikverketTrainMessageModel>(table);


            collection.ReplaceOne(filter: (u => u.EventId == record.EventId), options: new UpdateOptions { IsUpsert = true }, replacement: record);
        }

        public async Task Upsert(string table, StopPointNameMunicipalityModel record)
        {
            var collection = db.GetCollection<StopPointNameMunicipalityModel>(table);


           collection.ReplaceOne(filter: (u => u.Name == record.Name && u.MunicipalityName == record.MunicipalityName && u.SituationNumber == record.SituationNumber), options: new UpdateOptions { IsUpsert = true }, replacement: record);
        }
        public async Task Upsert(string table, VastTrafikModelTrafficSituation record)
        {
            var collection = db.GetCollection<VastTrafikModelTrafficSituation>(table);


            collection.ReplaceOne(filter: (u => u.SituationNumber == record.SituationNumber), options: new UpdateOptions { IsUpsert = true }, replacement: record);
        }

        
        public async Task UpsertAffectedLines(string table, StopPointNameMunicipalityModel record)
        {
            var collection = db.GetCollection<StopPointNameMunicipalityModel>(table);

            

            //IList<string> signList = new List<string>();

            //var dokuments = collection.Find(b => b.SituationNumber == record.SituationNumber).ToListAsync().Result;

            

            collection.UpdateMany(filter: (u => u.SituationNumber == record.SituationNumber), update: record.TransportAuthorityName);
            collection.UpdateMany(filter: (u => u.SituationNumber == record.SituationNumber), update: record.TransportAuthorityName);
        }
        

        public async Task FindRecord(string search)
        {

            // skriv in vilken model och tabell som ska hämtas
            var collection = db.GetCollection<TrafikverketTrainStationModel>(Trafikverket.station);

            // b är en instans av modellen som ska användas. jämförelse operatorer kan användas för sökningen (utöka med && and funktion möjligt???)
            // jämförelse sker mot "search" variablen som skickas in i funktionen
            // dokuments sparas i detta fallet som en lista för att möjliggöra en iterration ( foreach loop) 
            var dokuments = collection.Find(b => b.CountyNo == search).ToListAsync().Result;
            
            // exempel på retur av flera dokument
            foreach (var dokument in dokuments)
            {
                
                Console.WriteLine(dokument.AdvertisedLocationName);
            }

            // istället för en foreachloop kan man t.ex välja att returnera värdet ut från funktionen


        }
        public static IList<string> FindStationSign(string CountyNo)
        {

         
            var collection = db.GetCollection<TrafikverketTrainStationModel>(Trafikverket.station);

            IList<string> signList = new List<string>();

            var dokuments = collection.Find(b => b.CountyNo == CountyNo).ToListAsync().Result;

            // exempel på retur av flera dokument
            foreach (var dokument in dokuments)
            {

                signList.Add(dokument.LocationSignature);
            }

            return signList;
            // istället för en foreachloop kan man t.ex välja att returnera värdet ut från funktionen


        }




    }
}
