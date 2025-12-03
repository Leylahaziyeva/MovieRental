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
                    src.GenreTranslations.FirstOrDefault() != null ? src.GenreTranslations.First().Name : "N/A"));

            CreateMap<GenreTranslation, GenreTranslationViewModel>();
        }
    }
}