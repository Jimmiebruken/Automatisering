using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace mongoDBDemo
{
    public class TrafikverketModel
    {



        [BsonId]
        public Guid Id { get; set; }
        

        public string AdvertisedLocationName { get; set; }
        public string LocationSignature { get; set; }


    }
}
