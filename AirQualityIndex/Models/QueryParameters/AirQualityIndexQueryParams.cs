using System.ComponentModel.DataAnnotations;

namespace AirQualityIndex.Models.QueryParameters;

public class AirQualityIndexQueryParams
{
    [Required]
    public DateTime FromDate { get; set; }
    
    [Required]
    public DateTime ToDate { get; set; }
    
    [Required]
    public KeyValuePair<decimal, decimal> Coordinates { get; set; }
}