using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels;

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

            var featuredMovies = await _movieService.GetFeaturedMoviesAsync(languageId, 4);
            var latestMovies = await _movieService.GetLatestMoviesAsync(languageId, 4);
            var popularMovies = await _movieService.GetPopularMoviesAsync(languageId, 4);
            var upcomingMovies = await _movieService.GetUpcomingMoviesAsync(languageId, 4);

            var upcomingEvents = await _eventService.GetUpcomingEventsAsync(languageId);
            var featuredSports = await _sportService.GetFeaturedSportsAsync(4);

            var featuredEventsList = upcomingEvents
                .Where(e => e.IsFeatured)
                .Take(4)
                .ToList();

            return new HomeViewModel
            {
                Sliders = sliders.ToList(),
                Offers = offers.Take(4).ToList(),
                FeaturedMovies = featuredMovies.ToList(),
                LatestMovies = latestMovies.ToList(),
                PopularMovies = popularMovies.ToList(),
                UpcomingMovies = upcomingMovies.ToList(),
                Events = featuredEventsList,
                Sports = featuredSports.ToList()
            };
        }
    }
}