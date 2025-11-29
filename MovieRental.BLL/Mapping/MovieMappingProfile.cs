using AutoMapper;
using MovieRental.BLL.ViewModels.Movie;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Mapping
{
    public class MovieMappingProfile : Profile
    {
        public MovieMappingProfile()
        {
            CreateMap<Movie, MovieViewModel>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src =>
                    src.MovieTranslations.FirstOrDefault() != null
                        ? src.MovieTranslations.First().Title
                        : "N/A"))
                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src =>
                    src.MovieGenres.Select(mg => mg.Genre!.GenreTranslations.First().Name).ToList()))
                .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src => src.Currency.IsoCode))
                .ForMember(dest => dest.CurrencySymbol, opt => opt.MapFrom(src => src.Currency.Symbol))
                .ForMember(dest => dest.FormattedPrice, opt => opt.MapFrom(src =>
                    $"{src.Currency.Symbol}{src.RentalPrice:F2}"))
                .ForMember(dest => dest.Format, opt => opt.MapFrom(src => src.Format ?? "2D"))
                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language))
                .ForMember(dest => dest.LanguageName, opt => opt.MapFrom(src => src.Language.Name));


            CreateMap<Movie, MovieDetailsViewModel>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src =>
                    src.MovieTranslations.First().Title))
                .ForMember(dest => dest.Plot, opt => opt.MapFrom(src =>
                    src.MovieTranslations.First().Plot))
                .ForMember(dest => dest.DirectorNames, opt => opt.MapFrom(src =>
                    src.MovieTranslations.First().Director))  
                .ForMember(dest => dest.WriterNames, opt => opt.MapFrom(src =>
                    src.MovieTranslations.First().Writers))   
                .ForMember(dest => dest.CastNames, opt => opt.MapFrom(src =>
                    src.MovieTranslations.First().Cast))     

                .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
                .ForMember(dest => dest.FormattedPrice, opt => opt.MapFrom(src =>
                    $"{src.Currency.Symbol}{src.RentalPrice:F2}"))

                .ForMember(dest => dest.Language, opt => opt.MapFrom(src => src.Language))
                .ForMember(dest => dest.LanguageName, opt => opt.MapFrom(src => src.Language.Name))

                .ForMember(dest => dest.Genres, opt => opt.MapFrom(src =>
                    src.MovieGenres.Select(mg => mg.Genre)))

                .ForMember(dest => dest.Actors, opt => opt.MapFrom(src =>
                    src.MoviePersons
                        .Where(mp => mp.Role == MoviePersonRole.Actor && mp.IsActive)
                        .OrderBy(mp => mp.DisplayOrder)
                        .Select(mp => mp.Person)))
                .ForMember(dest => dest.Directors, opt => opt.MapFrom(src =>
                    src.MoviePersons
                        .Where(mp => mp.Role == MoviePersonRole.Director && mp.IsActive)
                        .Select(mp => mp.Person)))
                .ForMember(dest => dest.Writers, opt => opt.MapFrom(src =>
                    src.MoviePersons
                        .Where(mp => mp.Role == MoviePersonRole.Writer && mp.IsActive)
                        .Select(mp => mp.Person)))

                .ForMember(dest => dest.Images, opt => opt.MapFrom(src =>
                    src.MovieImages.Where(mi => mi.IsActive).OrderBy(mi => mi.DisplayOrder)))
                .ForMember(dest => dest.Videos, opt => opt.MapFrom(src =>
                    src.MovieVideos.Where(mv => mv.IsActive).OrderBy(mv => mv.DisplayOrder)))
                .ForMember(dest => dest.SocialLinks, opt => opt.MapFrom(src =>
                    src.MovieSocialLinks.Where(sl => sl.IsActive).OrderBy(sl => sl.DisplayOrder)));

            CreateMap<MovieCreateViewModel, Movie>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PosterImageUrl, opt => opt.Ignore()) 
                .ForMember(dest => dest.CoverImageUrl, opt => opt.Ignore())  
                .ForMember(dest => dest.VideoUrl, opt => opt.Ignore())       
                .ForMember(dest => dest.LovePercentage, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.VotesCount, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.MovieTranslations, opt => opt.Ignore())
                .ForMember(dest => dest.MovieGenres, opt => opt.Ignore())
                .ForMember(dest => dest.MoviePersons, opt => opt.Ignore())
                .ForMember(dest => dest.MovieImages, opt => opt.Ignore())
                .ForMember(dest => dest.MovieVideos, opt => opt.Ignore())
                .ForMember(dest => dest.MovieSocialLinks, opt => opt.Ignore());

            CreateMap<MovieUpdateViewModel, Movie>()
                .ForMember(dest => dest.PosterImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.CoverImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.VideoUrl, opt => opt.Ignore())
                .ForMember(dest => dest.MovieTranslations, opt => opt.Ignore())
                .ForMember(dest => dest.MovieGenres, opt => opt.Ignore())
                .ForMember(dest => dest.MoviePersons, opt => opt.Ignore())
                .ForMember(dest => dest.MovieImages, opt => opt.Ignore())
                .ForMember(dest => dest.MovieVideos, opt => opt.Ignore())
                .ForMember(dest => dest.MovieSocialLinks, opt => opt.Ignore());

            
            CreateMap<MovieTranslation, MovieTranslationViewModel>()
                .ForMember(dest => dest.DirectorNames, opt => opt.MapFrom(src => src.Director))
                .ForMember(dest => dest.WriterNames, opt => opt.MapFrom(src => src.Writers))
                .ForMember(dest => dest.CastNames, opt => opt.MapFrom(src => src.Cast));

            CreateMap<MovieTranslationCreateViewModel, MovieTranslation>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Director, opt => opt.MapFrom(src => src.DirectorNames))
                .ForMember(dest => dest.Writers, opt => opt.MapFrom(src => src.WriterNames))
                .ForMember(dest => dest.Cast, opt => opt.MapFrom(src => src.CastNames));

            CreateMap<MovieTranslationUpdateViewModel, MovieTranslation>()
                .ForMember(dest => dest.Director, opt => opt.MapFrom(src => src.DirectorNames))
                .ForMember(dest => dest.Writers, opt => opt.MapFrom(src => src.WriterNames))
                .ForMember(dest => dest.Cast, opt => opt.MapFrom(src => src.CastNames));

            CreateMap<MovieImage, MovieImageViewModel>();
            
            CreateMap<MovieVideo, MovieVideoViewModel>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src =>
                    src.MovieVideoTranslations.FirstOrDefault() != null
                        ? src.MovieVideoTranslations.First().Title
                        : ""))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src =>
                    src.MovieVideoTranslations.FirstOrDefault() != null
                        ? src.MovieVideoTranslations.First().Description
                        : ""))
                .ForMember(dest => dest.VideoType, opt => opt.MapFrom(src =>
                    src.VideoType.ToString()));
            
            CreateMap<MovieSocialLink, MovieSocialLinkViewModel>();
            CreateMap<MovieSocialLinkCreateDto, MovieSocialLink>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.MovieId, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true));
        }
    }
}