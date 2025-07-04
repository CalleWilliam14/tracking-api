namespace TrackingApi.Models;

public class Package
{
    public int Id { get; set; }

    public int TrackingNumber { get; set; }

    public string Name { get; set; } = String.Empty;

    public decimal Weight { get; set; }

    public int DestinationLocationId { get; set; }

    public Location DestinationLocation { get; set; } = null!;
}
