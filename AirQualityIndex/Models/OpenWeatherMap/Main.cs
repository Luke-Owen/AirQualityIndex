using System.Text.Json.Serialization;

namespace AirQualityIndex.Models.OpenWeatherMap;

public class Main
{
    [JsonPropertyName("aqi")]
    public int Aqi { get; set; }
}