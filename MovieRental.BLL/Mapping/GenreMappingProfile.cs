using AutoMapper;
using MovieRental.BLL.ViewModels.Genre;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Mapping
{
    public class GenreMappingProfile : Profile
    {
        public GenreMappingProfile()
        {
            CreateMap<Genre, GenreViewModel>()
             .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                 src.GenreTranslations.Select(t => t.Name).FirstOrDefault() ?? "N/A"));

            CreateMap<GenreTranslation, GenreTranslationViewModel>();

            CreateMap<GenreViewModel, GenreUpdateViewModel>()
                .ForMember(dest => dest.DefaultLanguageId, opt => opt.Ignore())
                .ForMember(dest => dest.Languages, opt => opt.Ignore())
                .ForMember(dest => dest.Translations, opt => opt.MapFrom(src =>
                    src.GenreTranslations));

            CreateMap<GenreTranslationViewModel, GenreTranslationUpdateViewModel>()
                .ForMember(dest => dest.LanguageId, opt => opt.MapFrom(src =>
                    src.Language != null ? src.Language.Id : 0));
        }
    }
}
