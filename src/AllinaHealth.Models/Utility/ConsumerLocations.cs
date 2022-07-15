using System;

namespace AllinaHealth.Models.Utility
{
    [Serializable]
    public class ConsumerLocations
    {
        public ConsumerLocations()
        {
            City = "";
            StateCode = "";
            ZipCode = "";
            Latitude = "";
            Longitude = "";
        }

        public string City { get; set; }
        public string StateCode { get; set; }
        public string ZipCode { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}