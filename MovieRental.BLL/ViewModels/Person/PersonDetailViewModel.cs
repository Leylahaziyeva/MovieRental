using MovieRental.BLL.ViewModels.Event;
using MovieRental.BLL.ViewModels.Movie;
using MovieRental.BLL.ViewModels.Sport;

namespace MovieRental.BLL.ViewModels.Person
{
    public class PersonDetailViewModel
    {
        public PersonViewModel? Person { get; set; }
        public List<MovieViewModel>? Movies { get; set; }
        public List<EventViewModel>? Events { get; set; }
        public List<SportViewModel>? Sports { get; set; }
    }
}