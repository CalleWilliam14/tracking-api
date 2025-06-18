using AutoMapper;
using TrackingApi.Models;
using TrackingApi.DTOs.Requests;
using TrackingApi.DTOs.Responses;

namespace TrackingApi.Profiles;

public class LocationProfile: Profile
{
    public LocationProfile()
    {
        CreateMap<Location, LocationDto>();

        CreateMap<Location, LocationResponseDto>();

        CreateMap<LocationDto, Location>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Packages, opt => opt.Ignore());
    }
}
