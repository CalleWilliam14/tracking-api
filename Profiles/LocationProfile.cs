using AutoMapper;
using tracking_api.Models;
using tracking_api.DTOs.Requests;
using tracking_api.DTOs.Responses;

namespace tracking_api.Profiles;

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
