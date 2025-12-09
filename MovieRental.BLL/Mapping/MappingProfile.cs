using AutoMapper;
using MovieRental.BLL.ViewModels.Currency;
using MovieRental.BLL.ViewModels.Language;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<Language, LanguageViewModel>()
              .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                  src.LanguageTranslations.FirstOrDefault() != null
                      ? src.LanguageTranslations.FirstOrDefault()!.Name
                      : src.IsoCode));

            CreateMap<LanguageCreateViewModel, Language>()
                .ForMember(dest => dest.LanguageTranslations, opt => opt.MapFrom(src => src.Translations));

            CreateMap<LanguageTranslationCreateViewModel, LanguageTranslation>();

            CreateMap<LanguageUpdateViewModel, Language>()
                .ForMember(dest => dest.LanguageTranslations, opt => opt.MapFrom(src => src.Translations));

            CreateMap<LanguageTranslationUpdateViewModel, LanguageTranslation>();

            CreateMap<Currency, CurrencyViewModel>()
              .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                  src.Translations.FirstOrDefault() != null
                      ? src.Translations.FirstOrDefault()!.Name
                      : src.IsoCode));

            CreateMap<CurrencyCreateViewModel, Currency>()
                .ForMember(dest => dest.Translations, opt => opt.MapFrom(src => src.Translations));

            CreateMap<CurrencyTranslationCreateViewModel, CurrencyTranslation>();

            CreateMap<CurrencyUpdateViewModel, Currency>()
                .ForMember(dest => dest.Translations, opt => opt.MapFrom(src => src.Translations));

            CreateMap<CurrencyTranslationUpdateViewModel, CurrencyTranslation>();
        }
    }
}