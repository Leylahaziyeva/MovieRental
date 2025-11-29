using AutoMapper;
using MovieRental.BLL.ViewModels.Sport;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Mapping
{
    public class SportMapperProfile : Profile
    {
        public SportMapperProfile()
        {
            CreateMap<Sport, SportViewModel>()
                .ForMember(dest => dest.FormattedPrice, opt => opt.Ignore()) 
                .ForMember(dest => dest.Players, opt => opt.MapFrom(src => src.Players))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
                .ForMember(dest => dest.Name, opt => opt.Ignore()) 
                .ForMember(dest => dest.Description, opt => opt.Ignore()) 
                .ForMember(dest => dest.Location, opt => opt.Ignore()); 

            CreateMap<SportCreateViewModel, Sport>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Currency, opt => opt.Ignore())
                .ForMember(dest => dest.SportTranslations, opt => opt.Ignore())
                .ForMember(dest => dest.Players, opt => opt.Ignore());

            CreateMap<SportUpdateViewModel, Sport>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Currency, opt => opt.Ignore())
                .ForMember(dest => dest.SportTranslations, opt => opt.Ignore())
                .ForMember(dest => dest.Players, opt => opt.Ignore());

            CreateMap<SportTranslation, SportTranslationViewModel>();

            CreateMap<SportTranslationCreateViewModel, SportTranslation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Sport, opt => opt.Ignore())
                .ForMember(dest => dest.Language, opt => opt.Ignore());

            CreateMap<SportTranslationUpdateViewModel, SportTranslation>()
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.Sport, opt => opt.Ignore())
                .ForMember(dest => dest.Language, opt => opt.Ignore());
        }
    }
}