using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace mongoDBDemo
{
    public class TrafikverketTrainAnnouncementModel
    {
        public string ActivityId { get; set; }
        public bool Canceled { get; set; }
        public DateTime AdvertisedTimeAtLocation { get; set; }
        public DateTime EstimatedTimeAtLocation { get; set; }
        public string Deviation { get; set; }
        public string LocationSignature { get; set; }
        public string InformationOwner { get; set; }
        public string FromLocation { get; set; }
        public string ToLocation { get; set; }

    }
    public class TrafikverketTrainStationModel
    {

        // Modell som används sen när data skickas till MongoDB, Värden som sätts här men inte skickas med till MongoDB får värdet null
        // !OBS! Packa upp array med forloop, array/list in i db skapar krash

        
        public string _id { get; }

        public string AdvertisedLocationName { get; set; }
        public string LocationSignature { get; set; }

        public float Longitude { get; set; }

        public float Latitude { get; set; }

        public string CountyNo { get; set; }



    }


    public class TrafikverketTrainMessageModel
    {


        public string EventId { get; set; }


        public string Header { get; set; }

        public string ExternalDescription { get; set; }

        public float Longitude { get; set; }

        public float Latitude { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public string ModifiedTime { get; set; }

        public List<string> AffectedLocation { get; set; }



    }
}
