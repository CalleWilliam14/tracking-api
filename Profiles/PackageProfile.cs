using AutoMapper;
using tracking_api.Models;
using tracking_api.DTOs.Requests;
using tracking_api.DTOs.Responses;

namespace tracking_api.Profiles;

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
