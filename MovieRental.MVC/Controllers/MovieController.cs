using Microsoft.AspNetCore.Mvc;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Movie;

namespace MovieRental.MVC.Controllers
{
    public class MovieController : Controller
    {
        private readonly IMovieService _movieService;
        private readonly ICookieService _cookieService;
        private readonly ISearchHistoryService _searchHistoryService;

        public MovieController(IMovieService movieService, ICookieService cookieService, ISearchHistoryService searchHistoryService)
        {
            _movieService = movieService;
            _cookieService = cookieService;
            _searchHistoryService = searchHistoryService;
        }

        [HttpGet("movies/filter")]
        public async Task<IActionResult> Filter(string? genre, string? language, string? sort, int? year, string? format, decimal? maxPrice, string? search)
        {
            var currentLanguageId = await _cookieService.GetLanguageIdAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                await _searchHistoryService.SaveSearchAsync(search, "Movie");
            }

            var filter = new MovieFilterViewModel
            {
                Genre = genre,
                Language = language,
                Sort = sort,
                Year = year,
                Format = format,
                MaxPrice = maxPrice,
                SearchQuery = search,
                CurrentLanguageId = currentLanguageId
            };

            var viewModel = await _movieService.GetFilteredMoviesAsync(filter);

            return View("Index", viewModel);
        }

        [HttpGet("movies/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var languageId = await _cookieService.GetLanguageIdAsync();
            var movie = await _movieService.GetMovieDetailsAsync(id, languageId);

            if (movie == null)
                return NotFound();

            return View(movie);
        }

        [HttpGet("movies")]
        public async Task<IActionResult> Index(string? search)
        {
            var languageId = await _cookieService.GetLanguageIdAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                await _searchHistoryService.SaveSearchAsync(search, "Movie");
            }

            var filter = new MovieFilterViewModel
            {
                CurrentLanguageId = languageId,
                SearchQuery = search
            };

            var viewModel = await _movieService.GetFilteredMoviesAsync(filter);

            return View(viewModel);
        }
    }
}