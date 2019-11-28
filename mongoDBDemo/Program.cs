using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace mongoDBDemo
{
    class Program
    {
        //test
        static async Task Main(string[] args)
        {
            MongoCRUD db = new MongoCRUD("admin");

            //startar en webclient
            ApiHelper.InitializeClientVastTrafik();


            //Kalla på metoden postrafikverket
            //TrafikverketProcessor.PostTrafikVerket(Query.TrainStation());

            VastTrafikProcessor.GetTrafficSituation();


            // ändra västrafiks anrop
            //db.InsertRecord("test", VastTrafik.LoadVastTrafik());
            //Console.WriteLine(VastTrafik.LoadVastTrafik());


            // Ligger endast för att blocka körningen från avslut ( för att kolla fel/ok medelanden)
            Console.ReadLine();

        }



    }


    // MongoCRUD sköter kontakten med DB, OBS! DB IP är satt till LAN! Public MONGODB IP är 78.67.178.206
    public class MongoCRUD
    {
        private IMongoDatabase db;

        public MongoCRUD(string database)
        {
            var client = new MongoClient("mongodb://localhost:27017/admin");
            db = client.GetDatabase(database);

        }

        public void InsertRecord<T>(string table, T record)
        {
            var collection = db.GetCollection<T>(table);
            collection.InsertOne(record);
        }

        public void DeleteRecord<T>(string table, Guid id)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);
        }

    }



}
