using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace mongoDBDemo
{
    class VastTrafikModel
    {

        [BsonId]
        public Guid Id { get; set; }
        

        public string SituationNumber { get; set; }
        public string CreationTime { get; set; }

    }
}
