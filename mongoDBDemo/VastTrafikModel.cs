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
        public string title { get; set; }

        

    }

    class VastTrafikModelLocation
    {
        public Point GeometryWGS84 { get; set; }
    }

}


