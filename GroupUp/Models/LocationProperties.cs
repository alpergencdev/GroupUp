using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GroupUp.Models
{
    public class LocationProperties
    {
        public string City { get; set; }
        public string CountryLongName { get; set; }
        public string CountryShortName { get; set; }
        public string Continent { get; set; }

        public double Lat { get; set; }
        public double Lng { get; set; }

        public bool IsMatchingLocation(string city, string country, string continent)
        {
            // Condition 1: Both city values and country values match.
            bool condition1 = city == this.City && country == this.CountryLongName;

            // Condition 2: City value does not matter, country values match, meaning the group is country-wide.
            bool condition2 = city == "-" && country == this.CountryLongName;

            // Condition 3: City and country values do not matter, continent values match, meaning the group is continent-wide.
            bool condition3 = city == "-" && country == "-" && continent == this.Continent;

            // Condition 4: All values do not matter, meaning the group is worldwide.
            bool condition4 = city == "-" && country == "-" && continent == "-";

            // if any of these conditions hold, then the given location
            // matches the location held inside this object.
            return condition1 || condition2 || condition3 || condition4;
        }
    }
}