namespace TrackingApi.DTOs.Responses;

public class LocationResponseDto
{
    public int Id { get; set; }
    
    public string Country { get; set; } = String.Empty;

    public string City { get; set; } = String.Empty;
}
