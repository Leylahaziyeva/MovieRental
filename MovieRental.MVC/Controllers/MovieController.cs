using Microsoft.AspNetCore.Mvc;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Movie;

namespace MovieRental.UI.Controllers
{
    public class MoviesController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly ICookieService _cookieService;

        public MoviesController(IMovieService movieService, ICookieService cookieService)
        {
            _movieService = movieService;
            _cookieService = cookieService;
        }
    
        public async Task<IActionResult> Index([FromQuery] MovieFilterViewModel filter)
        {
            var languageId = await _cookieService.GetLanguageIdAsync();
            filter.CurrentLanguageId = languageId;

            var result = await _movieService.GetFilteredMoviesAsync(filter);
            return View(result);
        }
       
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var languageId = await _cookieService.GetLanguageIdAsync();
            var movie = await _movieService.GetMovieDetailsAsync(id, languageId);

            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        public async Task<IActionResult> Featured()
        {
            var languageId = await _cookieService.GetLanguageIdAsync();
            var movies = await _movieService.GetFeaturedMoviesAsync(languageId, 20);

            var viewModel = new MoviesListViewModel
            {
                Movies = movies,
                PageTitle = "Featured Movies",
                Filter = new MovieFilterViewModel { CurrentLanguageId = languageId }
            };

            return View("Index", viewModel);
        }

        public async Task<IActionResult> Latest()
        {
            var languageId = await _cookieService.GetLanguageIdAsync();
            var movies = await _movieService.GetLatestMoviesAsync(languageId, 20);

            var viewModel = new MoviesListViewModel
            {
                Movies = movies,
                PageTitle = "Latest Movies",
                Filter = new MovieFilterViewModel { CurrentLanguageId = languageId }
            };

            return View("Index", viewModel);
        }
       
        public async Task<IActionResult> Popular()
        {
            var languageId = await _cookieService.GetLanguageIdAsync();
            var movies = await _movieService.GetPopularMoviesAsync(languageId, 20);

            var viewModel = new MoviesListViewModel
            {
                Movies = movies,
                PageTitle = "Popular Movies",
                Filter = new MovieFilterViewModel { CurrentLanguageId = languageId }
            };

            return View("Index", viewModel);
        }

        public async Task<IActionResult> Upcoming()
        {
            var languageId = await _cookieService.GetLanguageIdAsync();
            var movies = await _movieService.GetUpcomingMoviesAsync(languageId, 20);

            var viewModel = new MoviesListViewModel
            {
                Movies = movies,
                PageTitle = "Upcoming Movies",
                Filter = new MovieFilterViewModel { CurrentLanguageId = languageId }
            };

            return View("Index", viewModel);
        }
       
        // GET: /Movies/Search?q=avengers
        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
            {
                return RedirectToAction(nameof(Index));
            }

            var languageId = await _cookieService.GetLanguageIdAsync();

            var filter = new MovieFilterViewModel
            {
                SearchQuery = q.Trim(),
                CurrentLanguageId = languageId
            };

            var result = await _movieService.GetFilteredMoviesAsync(filter);
            result.PageTitle = $"Search Results for: {q}";

            return View("Index", result);
        }
            
        public async Task<IActionResult> ByGenre(string genreName)
        {
            if (string.IsNullOrWhiteSpace(genreName))
            {
                return RedirectToAction(nameof(Index));
            }

            var languageId = await _cookieService.GetLanguageIdAsync();

            var filter = new MovieFilterViewModel
            {
                Genre = genreName,
                CurrentLanguageId = languageId
            };

            var result = await _movieService.GetFilteredMoviesAsync(filter);
            result.PageTitle = $"{genreName} Movies";

            return View("Index", result);
        }

        public async Task<IActionResult> ByYear(int year)
        {
            if (year < 1900 || year > DateTime.Now.Year + 5)
            {
                return RedirectToAction(nameof(Index));
            }

            var languageId = await _cookieService.GetLanguageIdAsync();

            var filter = new MovieFilterViewModel
            {
                Year = year,
                CurrentLanguageId = languageId
            };

            var result = await _movieService.GetFilteredMoviesAsync(filter);
            result.PageTitle = $"Movies from {year}";

            return View("Index", result);
        }
       
        public async Task<IActionResult> LoadMore([FromQuery] MovieFilterViewModel filter)
        {
            var languageId = await _cookieService.GetLanguageIdAsync();
            filter.CurrentLanguageId = languageId;

            var result = await _movieService.GetFilteredMoviesAsync(filter);
            return PartialView("_MovieCardsPartial", result.Movies);    //_MovieCardsPartial yaradilmalidir
        }
    }
}