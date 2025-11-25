using AutoMapper;
using MovieRental.BLL.ViewModels.Actor;
using MovieRental.BLL.ViewModels.Genre;
using MovieRental.BLL.ViewModels.Movie;
using MovieRental.BLL.ViewModels.MovieImage;
using MovieRental.BLL.ViewModels.MovieSocialLink;
using MovieRental.BLL.ViewModels.MovieVideo;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Mappings
{
    public class MovieProfile : Profile
    {
        public MovieProfile()
        {
            CreateMap<Movie, MovieCardViewModel>()
                .ForMember(dest => dest.Title,
                    opt => opt.MapFrom(src => src.MovieTranslations.FirstOrDefault()!.Title))
                .ForMember(dest => dest.Genres,
                    opt => opt.MapFrom(src => src.MovieGenres
                        .Select(mg => mg.Genre.GenreTranslations.FirstOrDefault()!.Name)
                        .ToList()))
                .ForMember(dest => dest.ReleaseDate,
                    opt => opt.MapFrom(src => src.ReleaseDate))
                .ForMember(dest => dest.Language,
                    opt => opt.MapFrom(src => src.Language != null ? src.Language.Name : string.Empty));


            CreateMap<Movie, MovieDetailsViewModel>()
                .ForMember(dest => dest.Title,
                    opt => opt.MapFrom(src => src.MovieTranslations.FirstOrDefault()!.Title))
                .ForMember(dest => dest.Plot,
                    opt => opt.MapFrom(src => src.MovieTranslations.FirstOrDefault()!.Plot))
                .ForMember(dest => dest.Director,
                    opt => opt.MapFrom(src => src.MovieTranslations.FirstOrDefault()!.Director))
                .ForMember(dest => dest.Writers,
                    opt => opt.MapFrom(src => src.MovieTranslations.FirstOrDefault()!.Writers))
                .ForMember(dest => dest.Cast,
                    opt => opt.MapFrom(src => src.MovieTranslations.FirstOrDefault()!.Cast));

            CreateMap<Genre, GenreViewModel>()
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.GenreTranslations.FirstOrDefault()!.Name));

            CreateMap<MovieActor, ActorViewModel>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.Actor.Id))
                .ForMember(dest => dest.FullName,
                    opt => opt.MapFrom(src => src.Actor.ActorTranslations.FirstOrDefault()!.FullName))
                .ForMember(dest => dest.ProfileImageUrl,
                    opt => opt.MapFrom(src => src.Actor.ProfileImageUrl));

            CreateMap<MovieImage, MovieImageViewModel>();

            CreateMap<MovieVideo, MovieVideoViewModel>()
                .ForMember(dest => dest.Title,
                    opt => opt.MapFrom(src => src.MovieVideoTranslations.FirstOrDefault()!.Title))
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom(src => src.MovieVideoTranslations.FirstOrDefault()!.Description));

            CreateMap<MovieSocialLink, MovieSocialLinkViewModel>();
        }
    }
}