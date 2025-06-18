using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TrackingApi.Helpers;
using TrackingApi.Models;
using TrackingApi.DTOs.Requests;
using TrackingApi.DTOs.Responses;

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

    public PackageController(IMapper mapper)
    {
        _mapper = mapper;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var response = _mapper.Map<List<PackageResponseDto>>(_packages);

        return Ok(response);
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var package = _packages.FirstOrDefault(p => p.Id == id);

        if (package == null) return NotFound();

        var response = _mapper.Map<PackageResponseDto>(package);

        return Ok(response);
    }

    [HttpGet("track/{trackingNumber}")]
    public IActionResult GetByTrackingNumber(int trackingNumber)
    {
        var package = _packages.FirstOrDefault(p => p.TrackingNumber == trackingNumber);

        if (package == null) return NotFound();

        var response = _mapper.Map<PackageResponseDto>(package);

        return Ok(response);
    }

    [HttpPost]
    public IActionResult Create(PackageDto packageDto)

    {
        if (packageDto.Weight < 0)
            return BadRequest("El peso no puede ser negativo.");

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
}
