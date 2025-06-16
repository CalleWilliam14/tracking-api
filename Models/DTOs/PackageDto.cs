namespace tracking_api.Models.DTOs;

public class PackageDto
{
    public string Name { get; set; } = String.Empty;

    public decimal Weight { get; set; }

    public int DestinationLocationId { get; set; }
}
