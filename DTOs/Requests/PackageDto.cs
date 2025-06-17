namespace tracking_api.DTOs.Requests;

public class PackageDto
{
    public int TrackingNumber { get; set; }

    public string Name { get; set; } = String.Empty;

    public decimal Weight { get; set; }

    public int DestinationLocationId { get; set; }
}
