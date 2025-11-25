using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels;
using MovieRental.BLL.ViewModels.Movie;
using MovieRental.BLL.ViewModels.Event;
using MovieRental.BLL.ViewModels.Sport;

namespace MovieRental.BLL.Services.Implementations
{
    public class HomeManager : IHomeService
    {
        private readonly ISliderService _sliderService;
        private readonly IOfferService _offerService;
        private readonly IMovieService _movieService;
        private readonly IEventService _eventService;
        private readonly ISportService _sportService;
        private readonly ICookieService _cookieService;

        public HomeManager(
            ISliderService sliderService,
            IOfferService offerService,
            IMovieService movieService,
            IEventService eventService,
            ISportService sportService,
            ICookieService cookieService)
        {
            _sliderService = sliderService;
            _offerService = offerService;
            _movieService = movieService;
            _eventService = eventService;
            _sportService = sportService;
            _cookieService = cookieService;
        }

        public async Task<HomeViewModel> GetHomeViewModelAsync()
        {
            var language = await _cookieService.GetLanguageAsync();
            int languageId = language?.Id ?? 1;

            var sliders = await _sliderService.GetAllActiveSlidersAsync(languageId);

            var offers = await _offerService.GetActiveOffersAsync(languageId);

            var movieFilter = new MovieFilterViewModel
            {
                CurrentLanguageId = languageId
            };
            var allMoviesResult = await _movieService.GetFilteredMoviesAsync(movieFilter);

            var allMovies = allMoviesResult.Movies?.ToList() ?? new();

            // FeaturedMovies - IsFeatured = true olanlar
            var featuredMovies = allMovies.Where(m => m.IsFeatured).Take(4).ToList();

            // LatestMovies - Ən yeni əlavə olunanlar (CreatedAt-a görə)
            var latestMovies = allMovies.OrderByDescending(m => m.Id).Take(4).ToList();

            // PopularMovies - Ən çox vote alanlar
            var popularMovies = allMovies.OrderByDescending(m => m.VotesCount).Take(4).ToList();

            // UpcomingMovies - Gələcək tarixli filmlər
            var upcomingMovies = allMovies
                .Where(m => m.ReleaseDate > DateTime.Now)
                .OrderBy(m => m.ReleaseDate)
                .Take(4)
                .ToList();

            var eventFilter = new EventFilterViewModel
            {
                CurrentLanguageId = languageId
            };
            var eventsResult = await _eventService.GetFilteredEventsAsync(eventFilter);

            var sportFilter = new SportFilterViewModel
            {
                CurrentLanguageId = languageId
            };
            var sportsResult = await _sportService.GetFilteredSportsAsync(sportFilter);

            var homeViewModel = new HomeViewModel
            {
                Sliders = sliders.ToList(),
                Offers = offers.Take(4).ToList(),
                FeaturedMovies = featuredMovies,
                LatestMovies = latestMovies,
                PopularMovies = popularMovies,
                UpcomingMovies = upcomingMovies,
                Events = eventsResult.Events?.Take(4).ToList() ?? new(),
                Sports = sportsResult.Sports?.Take(4).ToList() ?? new()
            };

            return homeViewModel;
        }
    }
}