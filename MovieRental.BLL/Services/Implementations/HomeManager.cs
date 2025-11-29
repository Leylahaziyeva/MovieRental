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

            var slidersTask = _sliderService.GetAllActiveSlidersAsync(languageId);
            var offersTask = _offerService.GetActiveOffersAsync(languageId);

            var featuredMoviesTask = _movieService.GetFeaturedMoviesAsync(languageId, 4);
            var latestMoviesTask = _movieService.GetLatestMoviesAsync(languageId, 4);
            var popularMoviesTask = _movieService.GetPopularMoviesAsync(languageId, 4);
            var upcomingMoviesTask = _movieService.GetUpcomingMoviesAsync(languageId, 4);


            var upcomingEventsTask = _eventService.GetUpcomingEventsAsync(languageId);
            var featuredSportsTask = _sportService.GetFeaturedSportsAsync(4);

            await Task.WhenAll(
                slidersTask,
                offersTask,
                featuredMoviesTask,
                latestMoviesTask,
                popularMoviesTask,
                upcomingMoviesTask,
                upcomingEventsTask,
                featuredSportsTask
            );

            var sliders = await slidersTask;
            var offers = await offersTask;
            var featuredMovies = await featuredMoviesTask;
            var latestMovies = await latestMoviesTask;
            var popularMovies = await popularMoviesTask;
            var upcomingMovies = await upcomingMoviesTask;
            var upcomingEvents = await upcomingEventsTask;
            var featuredSports = await featuredSportsTask;

            var featuredEventsList = upcomingEvents
                .Where(e => e.IsFeatured)
                .Take(4)
                .ToList();

            var homeViewModel = new HomeViewModel
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

            return homeViewModel;
        }
    }
}