using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TrackingApi.Controllers;
using TrackingApi.Models;
using TrackingApi.DTOs.Requests;
using TrackingApi.DTOs.Responses;
using TrackingApi.Profiles;
using FluentAssertions;
using Xunit;
using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Logging;

namespace TrackingApi.Test.Controllers;

public class PackageControllerTest
{
    private readonly IMapper _mapper;
    private readonly PackageController _packageController;
    private readonly List<Location> _locations;
    private readonly List<Package> _packages;

    public PackageControllerTest()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PackageProfile>();
            cfg.AddProfile<LocationProfile>();
        });

        _mapper = config.CreateMapper();
        var loggerMock = new Mock<ILogger<PackageController>>();
        _packageController = new PackageController(_mapper, loggerMock.Object);

        _locations = new List<Location>
        {
            new Location { Id = 1, Country = "Bolivia", City = "La Paz" },
            new Location { Id = 2, Country = "Bolivia", City = "Cochabamba" }
        };
        _packages = new List<Package>
        {
            new Package { Id = 1, TrackingNumber = 1234, Name = "Zapatos", Weight = 12.2M, DestinationLocationId = 1, DestinationLocation = _locations[0] }
        };

        typeof(PackageController)
            .GetField("_locations", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(null, _locations);

        typeof(PackageController)
            .GetField("_packages", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)
            ?.SetValue(null, _packages);
    }

    [Fact]
    public void GetById_ReturnsNotFound_WhenPackageNotExists()
    {
        var result = _packageController.GetById(100);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public void GetById_ReturnsPackage_WhenPackageExists()
    {
        var package = new Package
        {
            Id = 2,
            Name = "Test",
            Weight = 23.3M,
            DestinationLocationId = 1
        };

        _packages.Add(package);

        var result = _packageController.GetById(2);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<PackageResponseDto>(okResult.Value);

        Assert.Equal(package.Name, returnValue.Name);
        Assert.Equal(package.Weight, returnValue.Weight);
    }

    [Fact]
    public void Create_ReturnsBadRequest_WhenPackageHasNegativeWeight()
    {
        var packageDto = new PackageDto
        {
            Name = "Test with negative weight",
            Weight = -23.3M,
            DestinationLocationId = 1
        };

        ValidateModel(packageDto, _packageController);

        var result = _packageController.Create(packageDto);
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public void Create_ReturnsCreateAtAction_WhenPackageIsOk()
    {
        var packageDto = new PackageDto
        {
            Name = "Test #2",
            Weight = 23.3M,
            DestinationLocationId = 1
        };

        ValidateModel(packageDto, _packageController);

        var result = _packageController.Create(packageDto);
        var createResult = Assert.IsType<CreatedAtActionResult>(result);
    }

    private void ValidateModel(object model, ControllerBase controller)
    {
        var validationContext = new ValidationContext(model, null, null);
        var results = new List<ValidationResult>();

        Validator.TryValidateObject(model, validationContext, results, true);

        foreach (var validationResult in results)
        {
            foreach (var memberName in validationResult.MemberNames)
            {
                controller.ModelState.AddModelError(memberName, validationResult.ErrorMessage);
            }
        }
    }
}
