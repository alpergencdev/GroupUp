using System.Collections.Generic;
// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace GroupUp.Models.LocationModels
{
    // created using https://json2csharp.com/
    // namings are not done according to pascal casing to successfully mirror the JSON objects returned by Google API, which use snake casing:
    public class PlusCode
    {
        public string compound_code { get; set; }
        public string global_code { get; set; }

    }

    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public List<string> types { get; set; }

    }

    public class Northeast
    {
        public double lat { get; set; }
        public double lng { get; set; }

    }

    public class Southwest
    {
        public double lat { get; set; }
        public double lng { get; set; }

    }

    public class Bounds
    {
        public Northeast northeast { get; set; }
        public Southwest southwest { get; set; }

    }

    public class Location
    {
        public double lat { get; set; }
        public double lng { get; set; }

    }

    public class Northeast2
    {
        public double lat { get; set; }
        public double lng { get; set; }

    }

    public class Southwest2
    {
        public double lat { get; set; }
        public double lng { get; set; }

    }

    public class Viewport
    {
        public Northeast2 northeast { get; set; }
        public Southwest2 southwest { get; set; }

    }

    public class Geometry
    {
        public Bounds bounds { get; set; }
        public Location location { get; set; }
        public string location_type { get; set; }
        public Viewport viewport { get; set; }

    }

    public class Result
    {
        public List<AddressComponent> address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public string place_id { get; set; }
        public List<string> types { get; set; }

    }

    public class Root
    {
        public PlusCode plus_code { get; set; }
        public List<Result> results { get; set; }
        public string status { get; set; }

    }
}