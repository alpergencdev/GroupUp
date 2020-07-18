using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GroupUp.Models.LocationModels
{
    public class LocationMethods
    {
        // This JSON String is used to identify continents from countries.
        public static string ContinentJsonString =
    "{\r\n  \"AD\": \"Europe\",\r\n  \"AE\": \"Asia\",\r\n  \"AF\": \"Asia\",\r\n  \"AG\": \"North America\",\r\n  \"AI\": \"North America\",\r\n  \"AL\": \"Europe\",\r\n  \"AM\": \"Asia\",\r\n  \"AN\": \"North America\",\r\n  \"AO\": \"Africa\",\r\n  \"AQ\": \"Antarctica\",\r\n  \"AR\": \"South America\",\r\n  \"AS\": \"Australia\",\r\n  \"AT\": \"Europe\",\r\n  \"AU\": \"Australia\",\r\n  \"AW\": \"North America\",\r\n  \"AZ\": \"Asia\",\r\n  \"BA\": \"Europe\",\r\n  \"BB\": \"North America\",\r\n  \"BD\": \"Asia\",\r\n  \"BE\": \"Europe\",\r\n  \"BF\": \"Africa\",\r\n  \"BG\": \"Europe\",\r\n  \"BH\": \"Asia\",\r\n  \"BI\": \"Africa\",\r\n  \"BJ\": \"Africa\",\r\n  \"BM\": \"North America\",\r\n  \"BN\": \"Asia\",\r\n  \"BO\": \"South America\",\r\n  \"BR\": \"South America\",\r\n  \"BS\": \"North America\",\r\n  \"BT\": \"Asia\",\r\n  \"BW\": \"Africa\",\r\n  \"BY\": \"Europe\",\r\n  \"BZ\": \"North America\",\r\n  \"CA\": \"North America\",\r\n  \"CC\": \"Asia\",\r\n  \"CD\": \"Africa\",\r\n  \"CF\": \"Africa\",\r\n  \"CG\": \"Africa\",\r\n  \"CH\": \"Europe\",\r\n  \"CI\": \"Africa\",\r\n  \"CK\": \"Australia\",\r\n  \"CL\": \"South America\",\r\n  \"CM\": \"Africa\",\r\n  \"CN\": \"Asia\",\r\n  \"CO\": \"South America\",\r\n  \"CR\": \"North America\",\r\n  \"CU\": \"North America\",\r\n  \"CV\": \"Africa\",\r\n  \"CX\": \"Asia\",\r\n  \"CY\": \"Asia\",\r\n  \"CZ\": \"Europe\",\r\n  \"DE\": \"Europe\",\r\n  \"DJ\": \"Africa\",\r\n  \"DK\": \"Europe\",\r\n  \"DM\": \"North America\",\r\n  \"DO\": \"North America\",\r\n  \"DZ\": \"Africa\",\r\n  \"EC\": \"South America\",\r\n  \"EE\": \"Europe\",\r\n  \"EG\": \"Africa\",\r\n  \"EH\": \"Africa\",\r\n  \"ER\": \"Africa\",\r\n  \"ES\": \"Europe\",\r\n  \"ET\": \"Africa\",\r\n  \"FI\": \"Europe\",\r\n  \"FJ\": \"Australia\",\r\n  \"FK\": \"South America\",\r\n  \"FM\": \"Australia\",\r\n  \"FO\": \"Europe\",\r\n  \"FR\": \"Europe\",\r\n  \"GA\": \"Africa\",\r\n  \"GB\": \"Europe\",\r\n  \"GD\": \"North America\",\r\n  \"GE\": \"Asia\",\r\n  \"GF\": \"South America\",\r\n  \"GG\": \"Europe\",\r\n  \"GH\": \"Africa\",\r\n  \"GI\": \"Europe\",\r\n  \"GL\": \"North America\",\r\n  \"GM\": \"Africa\",\r\n  \"GN\": \"Africa\",\r\n  \"GP\": \"North America\",\r\n  \"GQ\": \"Africa\",\r\n  \"GR\": \"Europe\",\r\n  \"GS\": \"Antarctica\",\r\n  \"GT\": \"North America\",\r\n  \"GU\": \"Australia\",\r\n  \"GW\": \"Africa\",\r\n  \"GY\": \"South America\",\r\n  \"HK\": \"Asia\",\r\n  \"HN\": \"North America\",\r\n  \"HR\": \"Europe\",\r\n  \"HT\": \"North America\",\r\n  \"HU\": \"Europe\",\r\n  \"ID\": \"Asia\",\r\n  \"IE\": \"Europe\",\r\n  \"IL\": \"Asia\",\r\n  \"IM\": \"Europe\",\r\n  \"IN\": \"Asia\",\r\n  \"IO\": \"Asia\",\r\n  \"IQ\": \"Asia\",\r\n  \"IR\": \"Asia\",\r\n  \"IS\": \"Europe\",\r\n  \"IT\": \"Europe\",\r\n  \"JE\": \"Europe\",\r\n  \"JM\": \"North America\",\r\n  \"JO\": \"Asia\",\r\n  \"JP\": \"Asia\",\r\n  \"KE\": \"Africa\",\r\n  \"KG\": \"Asia\",\r\n  \"KH\": \"Asia\",\r\n  \"KI\": \"Australia\",\r\n  \"KM\": \"Africa\",\r\n  \"KN\": \"North America\",\r\n  \"KP\": \"Asia\",\r\n  \"KR\": \"Asia\",\r\n  \"KW\": \"Asia\",\r\n  \"KY\": \"North America\",\r\n  \"KZ\": \"Asia\",\r\n  \"LA\": \"Asia\",\r\n  \"LB\": \"Asia\",\r\n  \"LC\": \"North America\",\r\n  \"LI\": \"Europe\",\r\n  \"LK\": \"Asia\",\r\n  \"LR\": \"Africa\",\r\n  \"LS\": \"Africa\",\r\n  \"LT\": \"Europe\",\r\n  \"LU\": \"Europe\",\r\n  \"LV\": \"Europe\",\r\n  \"LY\": \"Africa\",\r\n  \"MA\": \"Africa\",\r\n  \"MC\": \"Europe\",\r\n  \"MD\": \"Europe\",\r\n  \"ME\": \"Europe\",\r\n  \"MG\": \"Africa\",\r\n  \"MH\": \"Australia\",\r\n  \"MK\": \"Europe\",\r\n  \"ML\": \"Africa\",\r\n  \"MM\": \"Asia\",\r\n  \"MN\": \"Asia\",\r\n  \"MO\": \"Asia\",\r\n  \"MP\": \"Australia\",\r\n  \"MQ\": \"North America\",\r\n  \"MR\": \"Africa\",\r\n  \"MS\": \"North America\",\r\n  \"MT\": \"Europe\",\r\n  \"MU\": \"Africa\",\r\n  \"MV\": \"Asia\",\r\n  \"MW\": \"Africa\",\r\n  \"MX\": \"North America\",\r\n  \"MY\": \"Asia\",\r\n  \"MZ\": \"Africa\",\r\n  \"NA\": \"Africa\",\r\n  \"NC\": \"Australia\",\r\n  \"NE\": \"Africa\",\r\n  \"NF\": \"Australia\",\r\n  \"NG\": \"Africa\",\r\n  \"NI\": \"North America\",\r\n  \"NL\": \"Europe\",\r\n  \"NO\": \"Europe\",\r\n  \"NP\": \"Asia\",\r\n  \"NR\": \"Australia\",\r\n  \"NU\": \"Australia\",\r\n  \"NZ\": \"Australia\",\r\n  \"OM\": \"Asia\",\r\n  \"PA\": \"North America\",\r\n  \"PE\": \"South America\",\r\n  \"PF\": \"Australia\",\r\n  \"PG\": \"Australia\",\r\n  \"PH\": \"Asia\",\r\n  \"PK\": \"Asia\",\r\n  \"PL\": \"Europe\",\r\n  \"PM\": \"North America\",\r\n  \"PN\": \"Australia\",\r\n  \"PR\": \"North America\",\r\n  \"PS\": \"Asia\",\r\n  \"PT\": \"Europe\",\r\n  \"PW\": \"Australia\",\r\n  \"PY\": \"South America\",\r\n  \"QA\": \"Asia\",\r\n  \"RE\": \"Africa\",\r\n  \"RO\": \"Europe\",\r\n  \"RS\": \"Europe\",\r\n  \"RU\": \"Europe\",\r\n  \"RW\": \"Africa\",\r\n  \"SA\": \"Asia\",\r\n  \"SB\": \"Australia\",\r\n  \"SC\": \"Africa\",\r\n  \"SD\": \"Africa\",\r\n  \"SE\": \"Europe\",\r\n  \"SG\": \"Asia\",\r\n  \"SH\": \"Africa\",\r\n  \"SI\": \"Europe\",\r\n  \"SJ\": \"Europe\",\r\n  \"SK\": \"Europe\",\r\n  \"SL\": \"Africa\",\r\n  \"SM\": \"Europe\",\r\n  \"SN\": \"Africa\",\r\n  \"SO\": \"Africa\",\r\n  \"SR\": \"South America\",\r\n  \"ST\": \"Africa\",\r\n  \"SV\": \"North America\",\r\n  \"SY\": \"Asia\",\r\n  \"SZ\": \"Africa\",\r\n  \"TC\": \"North America\",\r\n  \"TD\": \"Africa\",\r\n  \"TF\": \"Antarctica\",\r\n  \"TG\": \"Africa\",\r\n  \"TH\": \"Asia\",\r\n  \"TJ\": \"Asia\",\r\n  \"TK\": \"Australia\",\r\n  \"TM\": \"Asia\",\r\n  \"TN\": \"Africa\",\r\n  \"TO\": \"Australia\",\r\n  \"TR\": \"Asia\",\r\n  \"TT\": \"North America\",\r\n  \"TV\": \"Australia\",\r\n  \"TW\": \"Asia\",\r\n  \"TZ\": \"Africa\",\r\n  \"UA\": \"Europe\",\r\n  \"UG\": \"Africa\",\r\n  \"US\": \"North America\",\r\n  \"UY\": \"South America\",\r\n  \"UZ\": \"Asia\",\r\n  \"VC\": \"North America\",\r\n  \"VE\": \"South America\",\r\n  \"VG\": \"North America\",\r\n  \"VI\": \"North America\",\r\n  \"VN\": \"Asia\",\r\n  \"VU\": \"Australia\",\r\n  \"WF\": \"Australia\",\r\n  \"WS\": \"Australia\",\r\n  \"YE\": \"Asia\",\r\n  \"YT\": \"Africa\",\r\n  \"ZA\": \"Africa\",\r\n  \"ZM\": \"Africa\",\r\n  \"ZW\": \"Africa\"\r\n}";

        /// <summary> 

        /// Uses the given latitude and longitude values to pinpoint the city, country and continent of the user.

        /// </summary> 
        public static void ReverseGeocode(double lat, double lng, ref LocationProperties locationProperties)
        {
            var latString = lat.ToString(CultureInfo.CreateSpecificCulture("en"));
            var lngString = lng.ToString(CultureInfo.CreateSpecificCulture("en"));
            // construct request uri to send to Google API.
            var requestUri =
                $"https://maps.googleapis.com/maps/api/geocode/json?latlng=" + latString +"," + lngString + "&key=AIzaSyC0UCs6UUsT2AsnSm-EZ9-wGjwAbbrBBI0&language=en&result_type=administrative_area_level_1&country";
            HttpClient client = new HttpClient();
            // Get response of Google API into a string.
            var responseString = client.GetStringAsync(requestUri);

            // Convert the JSON response of Google API into the Root object defined inside JSONClasses.cs
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(responseString.Result);
            var components = myDeserializedClass.results.First().address_components;
            // Get necessary values from root object.
            foreach (var component in components)
            {
                if (component.types.Contains("administrative_area_level_1"))
                {
                    locationProperties.City = component.long_name;
                }
                else if (component.types.Contains("country"))
                {
                    locationProperties.CountryShortName = component.short_name;
                    locationProperties.CountryLongName = component.long_name;
                    locationProperties.Continent = GetContinent(component.short_name);
                }
            }
        }

        // This method is used to get continent from 2-letter country codes provided by Google API.
        public static string GetContinent(string countryShort)
        {
            JObject continentObj = JObject.Parse(ContinentJsonString);
            JProperty prop = continentObj.Property(countryShort);
            return prop.Value.ToString();
        }
    }
}