using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TrackingApi.Controllers;
using TrackingApi.Models;
using TrackingApi.DTOs.Requests;
using TrackingApi.DTOs.Responses;
using TrackingApi.Profiles;
using FluentAssertions;

namespace TrackingApi.Test.Controllers;

public class PackageControllerTest
{
    private readonly IMapper _mapper;
    private readonly PackageController _packageController;

    public PackageControllerTest()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<PackageProfile>();
            cfg.AddProfile<LocationProfile>();
        });

        _mapper = config.CreateMapper();
        _packageController = new PackageController(_mapper);
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
        var packageDto = new PackageDto
        {
            Name = "Test",
            Weight = 23.3M,
            DestinationLocationId = 1
        };

        _packageController.Create(packageDto);

        var result = _packageController.GetById(2);
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnValue = Assert.IsType<PackageResponseDto>(okResult.Value);

        Assert.Equal(packageDto.Name, returnValue.Name);
        Assert.Equal(packageDto.Weight, returnValue.Weight);
    }
}
