using System.ComponentModel.DataAnnotations;

namespace AirQualityIndex.Models.QueryParameters;

public class CoordinatesQueryParams
{
    [Required]
    public string Latitude { get; set; }
    
    [Required]
    public string Longitude { get; set; }
}