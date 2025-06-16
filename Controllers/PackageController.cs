using Microsoft.AspNetCore.Mvc;
using tracking_api.Models;
using tracking_api.Models.DTOs;

namespace tracking_api.Controllers;

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
        new Package { Id = 1, Name = "Zapatos", Weight = 12.2M, DestinationLocationId = 1, DestinationLocation = _locations[0] }
    };

    [HttpGet]
    public IActionResult GetAll()
    {
        var response = _packages.Select(p => new PackageResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Weight = p.Weight,
            DestinationLocation = new LocationResponseDto
            {
                Id = p.DestinationLocation.Id,
                Country = p.DestinationLocation.Country,
                City = p.DestinationLocation.City
            }
        });

        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var package = _packages.FirstOrDefault(p => p.Id == id);

        if (package == null) return NotFound();

        var response = new PackageResponseDto
        {
            Id = package.Id,
            Name = package.Name,
            Weight = package.Weight,
            DestinationLocation = new LocationResponseDto
            {
                Id = package.DestinationLocation.Id,
                Country = package.DestinationLocation.Country,
                City = package.DestinationLocation.City
            }
        };

        return Ok(response);
    }

    [HttpPost]
    public IActionResult Create(PackageDto packageDto)
    {
        var destinationLocation = _locations.FirstOrDefault(l => l.Id == packageDto.DestinationLocationId);

        if (destinationLocation == null)
            return BadRequest("No se encontro el destino con ID: " + packageDto.DestinationLocationId);

        var newPackage = new Package
        {
            Id = _packages.Count + 1,
            Name = packageDto.Name,
            Weight = packageDto.Weight,
            DestinationLocationId = destinationLocation.Id,
            DestinationLocation = destinationLocation
        };

        _packages.Add(newPackage);

        var response = new PackageResponseDto
        {
            Id = newPackage.Id,
            Name = newPackage.Name,
            Weight = newPackage.Weight,
            DestinationLocation = new LocationResponseDto
            {
                Id = destinationLocation.Id,
                Country = destinationLocation.Country,
                City = destinationLocation.City
            }
        };

        return CreatedAtAction(nameof(GetById), new { id = newPackage.Id }, response);
    }
}
