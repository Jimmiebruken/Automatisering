using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace mongoDBDemo
{
    public class VastTrafikModelTrafficSituation
    {
        public string SituationNumber { get; set; }
        public DateTime CreationTime { get; set; }
        
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string Severity { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
     
        public List<AffectedStopPointsModel> AffectedStopPoints { get; set; }

    }

    public class AffectedStopPointsModel
    {
        public string StopPointGid { get; set; }
        public string Name { get; set; }
        public string MunicipalityName { get; set; }
        
    }

    public class StopPointNameMunicipalityModel
    {
        public string Name { get; set; }
        public string MunicipalityName { get; set; }
        public string SituationNumber { get; set; }
        public float Lon { get; set; }
        public float Lat { get; set; }
    }


    public class VastTrafikModelLocation
    {
        public string Name { get; set; }
        public float Lon { get; set; }
        public float Lat { get; set; }

    }

}


