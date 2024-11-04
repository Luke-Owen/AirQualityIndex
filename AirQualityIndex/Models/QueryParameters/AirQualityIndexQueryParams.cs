using System.ComponentModel.DataAnnotations;

namespace AirQualityIndex.Models.QueryParameters;

public class AirQualityIndexQueryParams : CoordinatesQueryParams
{
    [Required]
    public DateTime FromDate { get; set; }
    
    [Required]
    public DateTime ToDate { get; set; }
}