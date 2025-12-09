using AutoMapper;
using MovieRental.BLL.ViewModels.Location;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Mapping
{
    public class LocationMappingProfile : Profile
    {
        public LocationMappingProfile()
        {
            CreateMap<Location, LocationViewModel>()
             .ForMember(dest => dest.Name, opt => opt.Ignore())
             .ForMember(dest => dest.SportCount, opt => opt.Ignore())
             .ForMember(dest => dest.EventCount, opt => opt.Ignore());

            CreateMap<LocationCreateViewModel, Location>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Translations, opt => opt.Ignore())
                .ForMember(dest => dest.Sports, opt => opt.Ignore())
                .ForMember(dest => dest.Events, opt => opt.Ignore());

            CreateMap<LocationUpdateViewModel, Location>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.Translations, opt => opt.Ignore())
                .ForMember(dest => dest.Sports, opt => opt.Ignore())
                .ForMember(dest => dest.Events, opt => opt.Ignore());

            CreateMap<LocationTranslation, LocationTranslationViewModel>();
        }
    }
}