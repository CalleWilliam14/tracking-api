using System.ComponentModel.DataAnnotations;

namespace TrackingApi.DTOs.Requests;

public class PackageDto
{
    public int TrackingNumber { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre del paquete debe tener de 3 a 100 caracteres")]
    public string Name { get; set; } = String.Empty;

    [Range(0, 90, ErrorMessage = "El peso del paquete debe estar en un rango de 1 a 90 Kilogramos.")]
    public decimal Weight { get; set; }

    [Required]
    public int DestinationLocationId { get; set; }
}
