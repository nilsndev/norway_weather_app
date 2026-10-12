namespace norway_weather_api.DTOs
{
    public class WeatherResponseDto
    {
        public DateTime Time { get; set; }

        public double Temperature { get; set; }

        public string? WeatherSymbol { get; set; }

        public double WindSpeed { get; set; }

        public double WindDirection { get; set; }

        public double? PrecipitationAmount { get; set; }

        public double Humidity { get; set; }

        public double CloudCover { get; set; }
    }
}
