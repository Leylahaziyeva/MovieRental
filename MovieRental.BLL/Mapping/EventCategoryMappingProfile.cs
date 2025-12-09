using AutoMapper;
using MovieRental.BLL.ViewModels.EventCategory;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Mapping
{
    public class EventCategoryMappingProfile : Profile
    {
        public EventCategoryMappingProfile()
        {
            CreateMap<EventCategory, EventCategoryViewModel>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                    src.Translations.FirstOrDefault() != null
                        ? src.Translations.FirstOrDefault()!.Name
                        : string.Empty))
                .ForMember(dest => dest.EventCount, opt => opt.MapFrom(src =>
                    src.Events != null ? src.Events.Count : 0));

            CreateMap<EventCategoryTranslation, EventCategoryTranslationViewModel>();

        }
    }
}