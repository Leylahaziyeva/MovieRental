using MovieRental.BLL.ViewModels.Event;
using MovieRental.BLL.ViewModels.Movie;
using MovieRental.BLL.ViewModels.Offer;
using MovieRental.BLL.ViewModels.Sport;

namespace MovieRental.BLL.ViewModels
{
    public class HomeViewModel
    {
        public List<SliderViewModel> Sliders { get; set; } = [];
        public List<OfferViewModel> Offers { get; set; } = [];
        public List<MovieCardViewModel> FeaturedMovies { get; set; } = [];
        public List<MovieCardViewModel> LatestMovies { get; set; } = [];
        public List<MovieCardViewModel> PopularMovies { get; set; } = [];
        public List<MovieCardViewModel> UpcomingMovies { get; set; } = [];
        public List<EventViewModel> Events { get; set; } = new();
        public List<SportViewModel> Sports { get; set; } = new();
    }
}