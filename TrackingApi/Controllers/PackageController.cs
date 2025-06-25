using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TrackingApi.Helpers;
using TrackingApi.Models;
using TrackingApi.DTOs.Requests;
using TrackingApi.DTOs.Responses;
using Microsoft.Extensions.Logging;

namespace TrackingApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PackageController : ControllerBase
{
    private static List<Location> _locations = new()
    {
        new Location { Id = 1, Country = "Bolivia", City = "Cochabamba" },
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

    var elapsed = DateTime.UtcNow - start;
        // Log de rendimiento
        _logger.LogInformation("Bulk insert: {Count} paquetes en {Elapsed} ms", created, elapsed.TotalMilliseconds);

        return Ok(new { created, elapsed = elapsed.TotalMilliseconds });
    

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
}
