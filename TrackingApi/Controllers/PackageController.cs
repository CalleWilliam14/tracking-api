using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TrackingApi.Helpers;
using TrackingApi.Models;
using TrackingApi.DTOs.Requests;
using TrackingApi.DTOs.Responses;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.IO;

namespace TrackingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PackageController : ControllerBase
{
    private static List<Location> _locations = new()
    {
        new Location { Id = 1, Country = "Bolivia", City = "Comatttttttttttttttttt" },
        new Location { Id = 2, Country = "Bolivia", City = "La Paz" },
        new Location { Id = 3, Country = "Bolivia", City = "Santa Cruz" }
    };
    private static List<Package> _packages = new()
    {
        new Package { Id = 1, TrackingNumber = 1234, Name = "Zapatos", Weight = 12.2M, DestinationLocationId = 1, DestinationLocation = _locations[0] }
    };


    private readonly IMapper _mapper;
    private readonly ILogger<PackageController> _logger;

    public PackageController(IMapper mapper, ILogger<PackageController> logger)
    {
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        _logger.LogInformation("Solicitud recibida para obtener todos los paquetes.");
        var response = _mapper.Map<List<PackageResponseDto>>(_packages);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        _logger.LogInformation("Solicitud recibida para obtener el paquete con Id {Id}", id);
        var package = _packages.FirstOrDefault(p => p.Id == id);

        if (package == null) return NotFound();
        _logger.LogWarning("No se encontr� el paquete con Id {Id}", id);
        var response = _mapper.Map<PackageResponseDto>(package);

        return Ok(response);
    }

    [HttpGet("track/{trackingNumber}")]
    public IActionResult GetByTrackingNumber(int trackingNumber)
    {
        _logger.LogInformation("Solicitud recibida para obtener el paquete con TrackingNumber {TrackingNumber}", trackingNumber);
        var package = _packages.FirstOrDefault(p => p.TrackingNumber == trackingNumber);

        if (package == null){
            _logger.LogWarning("No se encontr� el paquete con TrackingNumber {TrackingNumber}", trackingNumber);
            return NotFound();
        } 
        var response = _mapper.Map<PackageResponseDto>(package);

        return Ok(response);
    }

    [HttpPost]
    public IActionResult Create(PackageDto packageDto)
    {
        var destinationLocation = _locations.FirstOrDefault(l => l.Id == packageDto.DestinationLocationId);

        if (destinationLocation == null)
            return BadRequest(ApiResponse.BadRequest(String.Format("Location with ID {0} doesn't exist.", packageDto.DestinationLocationId)));

        var newPackage = _mapper.Map<Package>(packageDto);

        newPackage.Id = _packages.Count + 1;
        newPackage.DestinationLocation = destinationLocation;

        _packages.Add(newPackage);

        var response = _mapper.Map<PackageResponseDto>(newPackage);

        return CreatedAtAction(nameof(GetById), new { id = newPackage.Id }, response);
    }



    [HttpPost("packages")]
    public IActionResult CreatePackages([FromBody] List<PackageDto> packages)
    {
        var created = 0;
        var start = DateTime.UtcNow;
        _logger.LogInformation("Solicitud recibida para crear {Count} paquetes en bulk.", packages.Count);
        foreach (var dto in packages)
        {
            if (dto.Weight < 0)
            {
                _logger.LogWarning("Intento de crear paquete con peso negativo: {Weight}", dto.Weight);
                continue;
            }
            var destinationLocation = _locations.FirstOrDefault(l => l.Id == dto.DestinationLocationId);
            if (destinationLocation == null)
            {
                _logger.LogWarning("Destino no encontrado para el paquete con TrackingNumber {TrackingNumber}", dto.TrackingNumber);
                continue;
            }
            var newPackage = _mapper.Map<Package>(dto);
            newPackage.Id = _packages.Count + 1;
            newPackage.DestinationLocation = destinationLocation;
            _packages.Add(newPackage);
            created++;
            _logger.LogInformation("Paquete creado exitosamente con Id {Id}", newPackage.Id);

        }

        var elapsed = DateTime.UtcNow - start;
        // Log de rendimiento
        _logger.LogInformation("se insertaron: {Count} paquetes en {Elapsed} ms", created, elapsed.TotalMilliseconds);

        return Ok(new { created, elapsed = elapsed.TotalMilliseconds });
    }

    [HttpGet("manifest/pdf")]
    public IActionResult GetManifestPdf()
    {
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        _logger.LogInformation("Solicitud recibida para generar el manifiesto PDF");
        try
        {
            // Obtener los paquetes actuales
            var packages = _packages;

            // Crear el PDF en memoria
            var stream = new MemoryStream();
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(30);
                    page.Header().Text("Manifiesto de Carga").FontSize(20).Bold().AlignCenter();
                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40); // ID
                            columns.RelativeColumn(2); // Nombre
                            columns.RelativeColumn(2); // Tracking
                            columns.RelativeColumn(2); // Destino
                            columns.RelativeColumn(1); // Peso
                        });
                        table.Header(header =>
                        {
                            header.Cell().Text("ID").Bold();
                            header.Cell().Text("Nombre").Bold();
                            header.Cell().Text("Tracking").Bold();
                            header.Cell().Text("Destino").Bold();
                            header.Cell().Text("Peso").Bold();
                        });
                        foreach (var p in packages)
                        {
                            table.Cell().Text(p.Id.ToString());
                            table.Cell().Text(p.Name);
                            table.Cell().Text(p.TrackingNumber.ToString());
                            table.Cell().Text($"{p.DestinationLocation?.City}, {p.DestinationLocation?.Country}");
                            table.Cell().Text(p.Weight.ToString("0.00"));
                        }
                    });
                    page.Footer().AlignCenter().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}");
                });
            }).GeneratePdf(stream);
            stream.Position = 0;
            _logger.LogInformation("PDF generado correctamente");
            return File(stream.ToArray(), "application/pdf", "manifiesto.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generando el manifiesto PDF");
            return StatusCode(500, $"Error generando PDF: {ex.Message}");
        }
    }

    [HttpGet("label/pdf/{id}")]
    public IActionResult GetLabelPdf(int id)
    {
        QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        _logger.LogInformation($"Solicitud recibida para generar etiqueta PDF para el paquete {id}");
        try
        {
            var package = _packages.FirstOrDefault(p => p.Id == id);
            if (package == null)
            {
                _logger.LogWarning("No se encontró el paquete con Id {Id}", id);
                return NotFound($"No se encontró el paquete con Id {id}");
            }

            var stream = new MemoryStream();
            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(200, 120);
                    page.Margin(10);
                    page.Content().Column(col =>
                    {
                        col.Item().Text($"Etiqueta de Envío").FontSize(14).Bold().AlignCenter();
                        col.Item().Text($"ID: {package.Id}").FontSize(10);
                        col.Item().Text($"Tracking: {package.TrackingNumber}").FontSize(10);
                        col.Item().Text($"Nombre: {package.Name}").FontSize(10);
                        col.Item().Text($"Destino: {package.DestinationLocation?.City}, {package.DestinationLocation?.Country}").FontSize(10);
                        col.Item().Text($"Peso: {package.Weight} kg").FontSize(10);
                    });
                });
            }).GeneratePdf(stream);
            stream.Position = 0;
            _logger.LogInformation("Etiqueta PDF generada correctamente para el paquete {Id}", id);
            return File(stream.ToArray(), "application/pdf", $"etiqueta_{id}.pdf");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generando la etiqueta PDF para el paquete {Id}", id);
            return StatusCode(500, $"Error generando PDF: {ex.Message}");
        }
    }
}
