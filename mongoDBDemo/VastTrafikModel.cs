using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace mongoDBDemo
{
    class VastTrafikModelTrafficSituation
    {
        public string situationNumber { get; set; }
        public DateTime creationTime { get; set; }
        
        public DateTime startTime { get; set; }

        public DateTime endTime { get; set; }

        public string severity { get; set; }

        public string title { get; set; }

        public string description { get; set; }

        public List<string> affectedStopPoints { get; set; }



    }

    class VastTrafikModelLocation
    {
        public Point GeometryWGS84 { get; set; }
    }

}


