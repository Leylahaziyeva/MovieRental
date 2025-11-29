using MovieRental.BLL.ViewModels.Event;
using MovieRental.BLL.ViewModels.Movie;
using MovieRental.BLL.ViewModels.Offer;
using MovieRental.BLL.ViewModels.Person;
using MovieRental.BLL.ViewModels.Sport;

namespace MovieRental.BLL.ViewModels
{
    public class HomeViewModel
    {
        public List<SliderViewModel> Sliders { get; set; } = [];
        public List<OfferViewModel> Offers { get; set; } = [];
        public List<MovieViewModel> FeaturedMovies { get; set; } = [];
        public List<MovieViewModel> LatestMovies { get; set; } = [];
        public List<MovieViewModel> PopularMovies { get; set; } = [];
        public List<MovieViewModel> UpcomingMovies { get; set; } = [];
        public List<EventViewModel> Events { get; set; } = [];
        public List<SportViewModel> Sports { get; set; } = [];
        public List<PersonViewModel>? PopularPeople { get; set; }
    }
}