using AutoMapper;
using MovieRental.BLL.ViewModels.Event;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Mapping
{
    public class EventMappingProfile : Profile
    {
        public EventMappingProfile()
        {
            CreateMap<Event, EventViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                    src.EventTranslations.FirstOrDefault() != null
                        ? src.EventTranslations.FirstOrDefault()!.Name
                        : string.Empty))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src =>
                    src.EventTranslations.FirstOrDefault() != null
                        ? src.EventTranslations.FirstOrDefault()!.Description
                        : string.Empty))
                .ForMember(dest => dest.Location, opt => opt.MapFrom(src =>
                    src.EventTranslations.FirstOrDefault() != null
                        ? src.EventTranslations.FirstOrDefault()!.Location
                        : string.Empty))
                .ForMember(dest => dest.Categories, opt => opt.MapFrom(src =>
                    src.EventTranslations.FirstOrDefault() != null
                        ? src.EventTranslations.FirstOrDefault()!.Categories
                        : null))
                .ForMember(dest => dest.Languages, opt => opt.MapFrom(src =>
                    src.EventTranslations.FirstOrDefault() != null
                        ? src.EventTranslations.FirstOrDefault()!.Languages
                        : null))
                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
                .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src =>
                    src.Currency != null ? src.Currency.IsoCode : null))
                .ForMember(dest => dest.CurrencySymbol, opt => opt.MapFrom(src =>
                    src.Currency != null ? src.Currency.Symbol : null))
                .ForMember(dest => dest.FormattedPrice, opt => opt.MapFrom(src =>
                    src.Price.HasValue
                        ? $"{(src.Currency != null ? src.Currency.Symbol : "")}{src.Price.Value:N2}"
                        : "Free"))
                .ForMember(dest => dest.Artists, opt => opt.MapFrom(src => src.Artists));

            CreateMap<EventCreateViewModel, Event>()
                .ForMember(dest => dest.EventTranslations, opt => opt.Ignore())
                .ForMember(dest => dest.Currency, opt => opt.Ignore())
                .ForMember(dest => dest.Artists, opt => opt.Ignore())
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<EventUpdateViewModel, Event>()
                .ForMember(dest => dest.EventTranslations, opt => opt.Ignore())
                .ForMember(dest => dest.Currency, opt => opt.Ignore())
                .ForMember(dest => dest.Artists, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<EventTranslation, EventTranslationViewModel>().ReverseMap();

            CreateMap<EventTranslationCreateViewModel, EventTranslation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Event, opt => opt.Ignore())
                .ForMember(dest => dest.Language, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            CreateMap<EventTranslationUpdateViewModel, EventTranslation>()
                .ForMember(dest => dest.Event, opt => opt.Ignore())
                .ForMember(dest => dest.Language, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }
}