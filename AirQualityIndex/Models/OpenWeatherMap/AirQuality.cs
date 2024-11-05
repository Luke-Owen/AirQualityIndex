using System.Text.Json.Serialization;

namespace AirQualityIndex.Models.OpenWeatherMap;

public class AirQuality
{
    [JsonPropertyName("main")]
    public Main Main { get; set; }
    
    [JsonPropertyName("dt")]
    public long Dt { get; set; }
}