namespace AirQualityIndex.Models;

using System.Text.Json.Serialization;

public class AirQuality
{
    [JsonPropertyName("aqi")]
    public int AQI { get; set; }
}

public class AirQualityWrapper
{
    [JsonPropertyName("main")]
    public AirQuality Main { get; set; }
}

public class AirQualityResponse
{
    [JsonPropertyName("list")]
    public List<AirQualityWrapper> List { get; set; }
}
