using System.Text.Json.Serialization;

namespace AirQualityIndex.Models.OpenWeatherMap;

public class AirQualityResponse
{
    [JsonPropertyName("list")]
    public List<AirQuality>? List { get; set; }
}