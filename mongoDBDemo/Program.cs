using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace mongoDBDemo
{
    class Program
    {

        static void Main(string[] args)
        {
            MongoCRUD db = new MongoCRUD("AddressBook");
            
            // startar en webclient
            ApiHelper.InitializeClient();
            string apikey = "'6bb34852b5b140e69f3eec606ea04220'";

            // sätter query i en string
            string quearyString = "<REQUEST>" +
                                        // Use your valid authenticationkey
                                        "<LOGIN authenticationkey=" + apikey + "/>" +
                                        "<QUERY objecttype='TrainStation' schemaversion='1'>" +
                                            "<FILTER>" +
                                                "<IN name='CountyNo' value='14'/>" +
                                            "</FILTER>" +
                                            "<INCLUDE>AdvertisedLocationName</INCLUDE>" +
                                            "<INCLUDE>LocationSignature</INCLUDE>" +
                                            "<INCLUDE>LocationInformationText</INCLUDE>" +
                                        "</QUERY>" +
                                    "</REQUEST>";

            // sätter query i formatet för http body request
            StringContent query = new StringContent(quearyString);


            //Console.WriteLine(TrafikverketProcessor.PostTrafikVerket(query));
            db.InsertRecord("test", TrafikverketProcessor.PostTrafikVerket(query));

            Console.ReadLine();

        }

        

    }

    // testmodell för att kolla db kontakt
    public class PersonModel
    {
        [BsonId]
        public Guid Id { get; set; }
        public string Firstname { get; set; }
        public string LastName { get; set; }
    }

    // MongoCRUD sköter kontakten med DB, OBS! DB IP är satt till LAN!
    public class MongoCRUD
    {
        private IMongoDatabase db;
        
        public MongoCRUD(string database)
        {
            var client = new MongoClient("mongodb://192.168.1.66:27017/admin");
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
