using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Movie;
using System.Security.Claims;

namespace MovieRental.UI.Controllers
{

    [Authorize]
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly ICookieService _cookieService;
        private readonly IWatchlistService _watchlistService;
        private readonly IGenreService _genreService;
        private readonly ILanguageService _languageService;
        private readonly IRentalService _rentalService;

        public MovieController(
            IMovieService movieService,
            ICookieService cookieService,
            IWatchlistService watchlistService,
            IGenreService genreService,
            ILanguageService languageService,
            IRentalService rentalService)
        {
            _movieService = movieService;
            _cookieService = cookieService;
            _watchlistService = watchlistService;
            _genreService = genreService;
            _languageService = languageService;
            _rentalService = rentalService;
        }


        public async Task<IActionResult> Index([FromQuery] MovieFilterViewModel filter)
        {
            var languageId = await _cookieService.GetLanguageIdAsync();
            filter.CurrentLanguageId = languageId;
            var result = await _movieService.GetFilteredMoviesAsync(filter);

            ViewBag.Genres = await _genreService.GetAllActiveAsync(languageId);
            ViewBag.Languages = await _languageService.GetAllWithTranslationsAsync(languageId);
            ViewBag.Formats = new List<string> { "2D", "3D", "4K", "IMAX" };
            ViewBag.Years = Enumerable.Range(1900, DateTime.Now.Year - 1900 + 1).Reverse().ToList();

            var currentYear = DateTime.Now.Year;
            ViewBag.Years = Enumerable.Range(1990, currentYear - 1990 + 1).OrderByDescending(y => y).ToList();

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

        public async Task<IActionResult> ByGenre(int genreId)
        {
            if (genreId <= 0)
            {
                return RedirectToAction(nameof(Index));
            }

            var languageId = await _cookieService.GetLanguageIdAsync();

            var filter = new MovieFilterViewModel
            {
                GenreId = genreId,
                CurrentLanguageId = languageId
            };

            var result = await _movieService.GetFilteredMoviesAsync(filter);

            var genre = await _genreService.GetByIdWithLanguageAsync(genreId, languageId);
            result.PageTitle = genre != null ? $"{genre.Name} Movies" : "Movies";

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

        [HttpPost]
        public async Task<IActionResult> ToggleWatchlist(int movieId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Please login first" });
            }

            var isInWatchlist = await _watchlistService.IsInWatchlistAsync(userId, movieId);

            if (isInWatchlist)
            {
                await _watchlistService.RemoveFromWatchlistAsync(userId, movieId);
                return Json(new { success = true, inWatchlist = false, message = "Removed from watchlist" });
            }
            else
            {
                await _watchlistService.AddToWatchlistAsync(userId, movieId);
                return Json(new { success = true, inWatchlist = true, message = "Added to watchlist" });
            }
        }

        public async Task<IActionResult> LoadMore([FromQuery] MovieFilterViewModel filter)
        {
            var languageId = await _cookieService.GetLanguageIdAsync();
            filter.CurrentLanguageId = languageId;

            var result = await _movieService.GetFilteredMoviesAsync(filter);
            return PartialView("_MovieCardsPartial", result.Movies);
        }

        public async Task<IActionResult> Watch(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account", new { returnUrl = Url.Action("Watch", "Movie", new { id }) });
            }

            var languageId = await _cookieService.GetLanguageIdAsync();
            var movie = await _movieService.GetMovieDetailsAsync(id, languageId);

            if (movie == null)
            {
                return NotFound();
            }

            var hasRental = await _rentalService.HasActiveRentalAsync(userId, id);

            if (!hasRental)
            {
                TempData["Error"] = "You need to rent this movie first!";
                return RedirectToAction("Details", new { id });
            }

            var rental = await _rentalService.GetActiveRentalForMovieAsync(userId, id);

            if (rental != null)
            {
                ViewBag.RentalInfo = rental;
            }

            return View(movie);
        }
    }
}