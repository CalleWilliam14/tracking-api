namespace TrackingApi.Models;

public class Location
{
    public int Id { get; set; }
    
    public string Country { get; set; } = String.Empty;

    public string City { get; set; } = String.Empty;

    public ICollection<Package> Packages { get; set; } = new List<Package>();
}
