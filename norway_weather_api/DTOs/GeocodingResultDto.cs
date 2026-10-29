using System.Text.Json.Serialization;

namespace norway_weather_api.DTOs
{
    public class GeocodingResultDto
    {
    
        public string DisplayName { get; set; } = string.Empty;
        

        public double Latitude { get; set; }
        
 
        public double Longitude { get; set; }

        public string CountryCode { get; set; } = string.Empty;
    }
}
