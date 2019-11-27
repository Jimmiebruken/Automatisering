using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace mongoDBDemo
{
    public class TrafikverketModel
    {

        // Modell som används sen när data skickas till MongoDB, Värden som sätts här men inte skickas med till MongoDB får värdet null

        [BsonId]
        public Guid Id { get; set; }
        

        public string AdvertisedLocationName { get; set; }
        public string LocationSignature { get; set; }

    }


    public class TrafikverketModelTrainMessage
    {

        public string EventId { get; set; }

        public string StationsSignatur { get; set; }

        public string Header { get; set; }

        public string ExternalDescription { get; set; }

        public string GeometryWGS84 { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string ModifiedTime { get; set; }

        public Array TrafficImpactAffectedLocation { get; set; }



    }
}
