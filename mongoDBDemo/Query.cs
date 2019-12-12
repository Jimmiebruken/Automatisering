using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace mongoDBDemo
{
    class Query
    {
        public static string apikey = "'6bb34852b5b140e69f3eec606ea04220'";

        public static StringContent TrainAnnouncement(string locationSignature)
        {

            // sätter query i en string
            string quearyString = "<REQUEST>" +
                                         // Use your valid authenticationkey
                                         "<LOGIN authenticationkey=" + apikey + "/>" +
                        "<QUERY objecttype='TrainAnnouncement' schemaversion='1.6'>" +
                                            "<FILTER>" +
                                            "<AND>"+
                                            "<EQ name = 'ActivityType' value = 'Avgang' />" +
                                            "<EQ name = 'LocationSignature' value = '"+ locationSignature +"'/>" +
                                        "<OR>" +
                                        "<AND>"+
      
                                          "<GT name = 'AdvertisedTimeAtLocation' value = '$dateadd(-00:15:00)' />"+
                                             "<LT name = 'AdvertisedTimeAtLocation' value = '$dateadd(14:00:00)' />"+
            
                                          "</AND>"+
                                          "<AND>"+
                                                "<LT name = 'AdvertisedTimeAtLocation' value = '$dateadd(00:30:00)' />"+
                                                   "<GT name = 'EstimatedTimeAtLocation' value = '$dateadd(-00:15:00)' />"+
                                                "</AND>"+
                                          "</OR>"+
                                          "</AND>" +
                                            "</FILTER>" +
                                            "<INCLUDE>ActivityId</INCLUDE>"+
                                            "<INCLUDE>Canceled</INCLUDE>"+
                                            "<INCLUDE>AdvertisedTimeAtLocation</INCLUDE>"+
                                            "<INCLUDE>EstimatedTimeAtLocation</INCLUDE>"+
                                            "<INCLUDE>Deviation</INCLUDE>"+
                                            "<INCLUDE>LocationSignature</INCLUDE>"+
                                            "<INCLUDE>InformationOwner</INCLUDE>"+
                                            "<INCLUDE>FromLocation</INCLUDE>"+
                                            "<INCLUDE>ToLocation</INCLUDE>"+
                                        "</QUERY>" +
                                    "</REQUEST>";

            // sätter query i formatet för http body request
            StringContent query = new StringContent(quearyString);

            return query;
        }



        public static StringContent TrainStation()
        {


            // sätter query i en string
            string quearyString = "<REQUEST>" +
                                         // Use your valid authenticationkey
                                         "<LOGIN authenticationkey=" + apikey + "/>" +
                        "<QUERY objecttype='TrainStation' schemaversion='1'>" +
                                            "<FILTER>" +
                                            "</FILTER>" +
                                        "</QUERY>" +
                                    "</REQUEST>";

            // sätter query i formatet för http body request
            StringContent query = new StringContent(quearyString);

            return query;

        }
        public static StringContent TrainMessage()
        {

            string quearyString = "<REQUEST>" +
                                   // Use your valid authenticationkey
                                   "<LOGIN authenticationkey=" + apikey + "/>" +
                                   "<QUERY objecttype='TrainMessage' schemaversion='1.3'>" +
                                       "<FILTER>" +
                                           "<EQ name = 'CountyNo' value = '14' />"+
                                          "</FILTER>" +
                                   "</QUERY>" +
                               "</REQUEST>";

            // sätter query i formatet för http body request
            StringContent query = new StringContent(quearyString);


            return query;
        }
    }
}
