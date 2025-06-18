using AutoMapper;
using TrackingApi.Models;
using TrackingApi.DTOs.Requests;
using TrackingApi.DTOs.Responses;

namespace TrackingApi.Profiles;

public class PackageProfile: Profile
{
    public PackageProfile()
    {
        CreateMap<Package, PackageDto>();

        CreateMap<Package, PackageResponseDto>()
            .ForMember(dest => dest.DestinationLocation,
                    opt => opt.MapFrom(src => src.DestinationLocation));
        
        CreateMap<PackageDto, Package>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DestinationLocationId,
                    opt => opt.MapFrom(src => src.DestinationLocationId))
            .ForMember(dest => dest.DestinationLocation, opt => opt.Ignore());
    }
}
