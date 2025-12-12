using AutoMapper;
using MovieRental.BLL.ViewModels.Event;
using MovieRental.BLL.ViewModels.Person;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Mapping
{
    public class EventMappingProfile : Profile
    {
        public EventMappingProfile()
        {
            // Event -> EventViewModel
            CreateMap<Event, EventViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                    src.EventTranslations.FirstOrDefault() != null
                        ? src.EventTranslations.FirstOrDefault()!.Name
                        : string.Empty))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src =>
                    src.EventTranslations.FirstOrDefault() != null
                        ? src.EventTranslations.FirstOrDefault()!.Description
                        : string.Empty))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src =>
                    src.EventCategory != null && src.EventCategory.Translations.Any()
                        ? src.EventCategory.Translations.FirstOrDefault()!.Name
                        : null))
                .ForMember(dest => dest.LocationName, opt => opt.MapFrom(src =>
                    src.Location != null && src.Location.Translations.Any()
                        ? src.Location.Translations.FirstOrDefault()!.Name
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
                .ForMember(dest => dest.Artists, opt => opt.MapFrom(src =>
                    src.Artists != null ? src.Artists : new List<Person>()));

            // Person -> PersonViewModel (for Artists)
            CreateMap<Person, PersonViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                    src.PersonTranslations != null && src.PersonTranslations.Any()
                        ? src.PersonTranslations.FirstOrDefault()!.Name
                        : string.Empty))
                .ForMember(dest => dest.Biography, opt => opt.MapFrom(src =>
                    src.PersonTranslations != null && src.PersonTranslations.Any()
                        ? src.PersonTranslations.FirstOrDefault()!.Biography
                        : string.Empty))
                .ForMember(dest => dest.PersonType, opt => opt.MapFrom(src => src.PersonType))
                .ForMember(dest => dest.PersonTypeDisplay, opt => opt.Ignore())
                .ForMember(dest => dest.KnownFor, opt => opt.Ignore());

            // EventCreateViewModel -> Event
            CreateMap<EventCreateViewModel, Event>()
                .ForMember(dest => dest.EventTranslations, opt => opt.Ignore())
                .ForMember(dest => dest.Currency, opt => opt.Ignore())
                .ForMember(dest => dest.EventCategory, opt => opt.Ignore())
                .ForMember(dest => dest.Location, opt => opt.Ignore())
                .ForMember(dest => dest.Artists, opt => opt.Ignore()) // Will be handled manually
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            // EventUpdateViewModel -> Event
            CreateMap<EventUpdateViewModel, Event>()
                .ForMember(dest => dest.EventTranslations, opt => opt.Ignore())
                .ForMember(dest => dest.Currency, opt => opt.Ignore())
                .ForMember(dest => dest.EventCategory, opt => opt.Ignore())
                .ForMember(dest => dest.Location, opt => opt.Ignore())
                .ForMember(dest => dest.Artists, opt => opt.Ignore()) // Will be handled manually
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            // EventTranslationCreateViewModel -> EventTranslation
            CreateMap<EventTranslationCreateViewModel, EventTranslation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Event, opt => opt.Ignore())
                .ForMember(dest => dest.Language, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());

            // EventTranslationUpdateViewModel -> EventTranslation
            CreateMap<EventTranslationUpdateViewModel, EventTranslation>()
                .ForMember(dest => dest.Event, opt => opt.Ignore())
                .ForMember(dest => dest.Language, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore());
        }
    }
}