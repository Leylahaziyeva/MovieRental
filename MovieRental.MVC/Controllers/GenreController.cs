using Microsoft.AspNetCore.Mvc;
using MovieRental.BLL.Services.Contracts;

namespace MovieRental.MVC.Controllers
{
    public class GenreController : Controller
    {
        private readonly IGenreService _genreService;
        private readonly IMovieService _movieService;
        private readonly ICookieService _cookieService;

        public GenreController(
            IGenreService genreService,
            IMovieService movieService,
            ICookieService cookieService)
        {
            _genreService = genreService;
            _movieService = movieService;
            _cookieService = cookieService;
        }

        public async Task<IActionResult> Index()
        {
            var languageId = await _cookieService.GetLanguageIdAsync();
            var genres = await _genreService.GetAllActiveAsync(languageId);

            return View(genres);
        }

        public async Task<IActionResult> Popular(int count = 20)
        {
            var languageId = await _cookieService.GetLanguageIdAsync();
            var genres = await _genreService.GetPopularGenresAsync(languageId, count);

            return View("Index", genres);
        }

        // GET: /Genre/Browse/5 (Genre ID ilə)
        // və ya /Genre/Browse/action (Genre Name ilə)
        public async Task<IActionResult> Browse(string id)
        {
            var languageId = await _cookieService.GetLanguageIdAsync();

            if (int.TryParse(id, out int genreId))
            {
                var genre = await _genreService.GetByIdWithLanguageAsync(genreId, languageId);
                if (genre == null)
                    return NotFound();

                var filter = new MovieRental.BLL.ViewModels.Movie.MovieFilterViewModel
                {
                    GenreId = genre.Id,
                    CurrentLanguageId = languageId
                };

                var movies = await _movieService.GetFilteredMoviesAsync(filter);
                movies.PageTitle = $"{genre.Name} Movies";

                return View(movies);
            }
            else
            {
                var genres = await _genreService.SearchByNameAsync(id, languageId);
                var genre = genres.FirstOrDefault();

                if (genre == null)
                    return NotFound();

                var filter = new MovieRental.BLL.ViewModels.Movie.MovieFilterViewModel
                {
                    GenreId = genre.Id,
                    CurrentLanguageId = languageId
                };

                var movies = await _movieService.GetFilteredMoviesAsync(filter);
                movies.PageTitle = $"{genre.Name} Movies";

                return View(movies);
            }
        }

        public async Task<IActionResult> Search(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return RedirectToAction(nameof(Index));

            var languageId = await _cookieService.GetLanguageIdAsync();
            var genres = await _genreService.SearchByNameAsync(q.Trim(), languageId);

            ViewBag.SearchQuery = q;
            return View("Index", genres);
        }

        [HttpGet]
        public async Task<IActionResult> GetGenreMovieCount(int id)
        {
            var count = await _genreService.GetGenreMovieCountAsync(id);
            return Json(new { success = true, count });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var languageId = await _cookieService.GetLanguageIdAsync();
            var genres = await _genreService.GetAllActiveAsync(languageId);

            var result = genres.Select(g => new
            {
                id = g.Id,
                name = g.Name
            });

            return Json(result);
        }
    }
}
