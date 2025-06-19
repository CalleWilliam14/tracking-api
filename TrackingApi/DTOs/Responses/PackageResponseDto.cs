namespace TrackingApi.DTOs.Responses;

public class PackageResponseDto
{
    public int Id { get; set; }

    public int TrackingNumber { get; set; }

    public string Name { get; set; } = String.Empty;

    public decimal Weight { get; set; }

    public LocationResponseDto DestinationLocation { get; set; } = null!;
}
