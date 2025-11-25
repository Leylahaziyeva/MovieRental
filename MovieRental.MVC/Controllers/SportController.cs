using Microsoft.AspNetCore.Mvc;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Sport;

namespace MovieRental.MVC.Controllers
{
    public class SportController : Controller
    {
        private readonly ISportService _sportService;
        private readonly ICookieService _cookieService;

        public SportController(ISportService sportService, ICookieService cookieService)
        {
            _sportService = sportService;
            _cookieService = cookieService;
        }

        public async Task<IActionResult> Index(string? search, string? location, DateTime? fromDate, DateTime? toDate)
        {
            var currentLanguageId = await _cookieService.GetLanguageIdAsync();

            var filter = new SportFilterViewModel
            {
                CurrentLanguageId = currentLanguageId,
                SearchQuery = search,
                Location = location,
                FromDate = fromDate,
                ToDate = toDate
            };

            var viewModel = await _sportService.GetFilteredSportsAsync(filter);

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var currentLanguageId = await _cookieService.GetLanguageIdAsync();
            var sport = await _sportService.GetSportByIdWithTranslationsAsync(id, currentLanguageId);

            if (sport == null)
                return NotFound();

            return View(sport);
        }
    }
}