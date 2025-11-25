using AutoMapper;
using MovieRental.BLL.ViewModels.Sport;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Mapping
{
    public class SportProfile : Profile
    {
        public SportProfile()
        {
            CreateMap<Sport, SportViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                    src.SportTranslations.FirstOrDefault() != null
                        ? src.SportTranslations.FirstOrDefault()!.Name
                        : string.Empty))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src =>
                    src.SportTranslations.FirstOrDefault() != null
                        ? src.SportTranslations.FirstOrDefault()!.Description
                        : string.Empty))
                .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src =>
                    src.Currency != null ? src.Currency.IsoCode : null))
                .ForMember(dest => dest.CurrencySymbol, opt => opt.MapFrom(src =>
                    src.Currency != null ? src.Currency.Symbol : null));

            CreateMap<SportCreateViewModel, Sport>()
                .ForMember(dest => dest.SportTranslations, opt => opt.Ignore());

            CreateMap<SportUpdateViewModel, Sport>()
                .ForMember(dest => dest.SportTranslations, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}