using System.ComponentModel.DataAnnotations;

namespace TrackingApi.DTOs.Requests;

public class PackageDto
{
    public int TrackingNumber { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = String.Empty;

    [Range(0, 90)]
    public decimal Weight { get; set; }

    [Required]
    public int DestinationLocationId { get; set; }
}
