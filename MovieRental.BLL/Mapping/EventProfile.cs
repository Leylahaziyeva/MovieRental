using AutoMapper;
using MovieRental.BLL.ViewModels.Event;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Mapping
{
    public class EventProfile : Profile
    {
        public EventProfile()
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
                .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src =>
                    src.Currency != null ? src.Currency.IsoCode : null))
                .ForMember(dest => dest.CurrencySymbol, opt => opt.MapFrom(src =>
                    src.Currency != null ? src.Currency.Symbol : null));

            CreateMap<EventCreateViewModel, Event>()
                .ForMember(dest => dest.EventTranslations, opt => opt.Ignore());

            CreateMap<EventUpdateViewModel, Event>()
                .ForMember(dest => dest.EventTranslations, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore());
        }
    }
}