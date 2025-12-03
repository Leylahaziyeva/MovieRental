using AutoMapper;
using MovieRental.BLL.ViewModels.Offer;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Mapping
{
    public class OfferMappingProfile : Profile
    {
        public OfferMappingProfile()
        {
            CreateMap<Offer, OfferViewModel>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src =>
                    src.OfferTranslations.FirstOrDefault() != null
                        ? src.OfferTranslations.FirstOrDefault()!.Title
                        : string.Empty))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src =>
                    src.OfferTranslations.FirstOrDefault() != null
                        ? src.OfferTranslations.FirstOrDefault()!.Description
                        : string.Empty))
                .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src =>
                    src.Currency != null ? src.Currency.IsoCode : null))
                .ForMember(dest => dest.CurrencySymbol, opt => opt.MapFrom(src =>
                    src.Currency != null ? src.Currency.Symbol : null))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency));

            CreateMap<OfferCreateViewModel, Offer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.OfferTranslations, opt => opt.Ignore())
                .ForMember(dest => dest.Currency, opt => opt.Ignore());

            CreateMap<OfferUpdateViewModel, Offer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ImageUrl, opt => opt.Condition(src => src.ImageFile == null))
                .ForMember(dest => dest.LogoUrl, opt => opt.Condition(src => src.LogoFile == null))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.OfferTranslations, opt => opt.Ignore())
                .ForMember(dest => dest.Currency, opt => opt.Ignore());

            CreateMap<OfferTranslation, OfferTranslationViewModel>()
                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language));
        }
    }
}