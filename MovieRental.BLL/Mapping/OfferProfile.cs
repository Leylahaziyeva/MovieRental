using AutoMapper;
using MovieRental.BLL.ViewModels.Offer;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Mapping
{
    public class OfferProfile : Profile
    {
        public OfferProfile()
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
                    src.Currency != null ? src.Currency.Symbol : null));

            CreateMap<OfferCreateViewModel, Offer>()
                .ForMember(dest => dest.OfferTranslations, opt => opt.Ignore());

            CreateMap<OfferUpdateViewModel, Offer>()
                .ForMember(dest => dest.OfferTranslations, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}