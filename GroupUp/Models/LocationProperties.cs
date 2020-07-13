using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupUp.Models
{
    public class LocationProperties
    {
        public string City { get; set; }
        public string Country { get; set; }
        public string Continent { get; set; }

        public double Lat { get; set; }
        public double Lng { get; set; }
    }
}