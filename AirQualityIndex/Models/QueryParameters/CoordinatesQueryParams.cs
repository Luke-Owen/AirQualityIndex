using System.ComponentModel.DataAnnotations;

namespace AirQualityIndex.Models.QueryParameters;

public class CoordinatesQueryParams
{
    [Required]
    public decimal Latitude { get; set; }
    
    [Required]
    public decimal Longitude { get; set; }
}