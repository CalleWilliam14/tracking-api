namespace tracking_api.Models;

public class Package
{
    public int Id { get; set; }

    public string Name { get; set; } = String.Empty;

    public decimal Weight { get; set; }

    public int DestinationLocationId { get; set; }

    public Location DestinationLocation { get; set; }
}
