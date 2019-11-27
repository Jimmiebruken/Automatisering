using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace mongoDBDemo
{
    class Query
    {
        public static string apikey = "'6bb34852b5b140e69f3eec606ea04220'";

        public static StringContent TrainStation(){
        

        // sätter query i en string
        string quearyString = "<REQUEST>" +
                                     // Use your valid authenticationkey
                                     "<LOGIN authenticationkey=" + apikey +"/>" +
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

            return query;

    }
        public static StringContent TrainMessage()
        {

            string quearyString = "<REQUEST>" +
                                   // Use your valid authenticationkey
                                   "<LOGIN authenticationkey=" + apikey + "/>" +
                                   "<QUERY objecttype='TrainMessage' schemaversion='1.3'>" +
                                       "<FILTER>" +
                                           "<IN name='CountyNo' value='14'/>" +
                                       "</FILTER>" +
                                   "</QUERY>" +
                               "</REQUEST>";

            // sätter query i formatet för http body request
            StringContent query = new StringContent(quearyString);


            return query;
        }
    }
}
