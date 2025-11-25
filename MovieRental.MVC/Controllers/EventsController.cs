using Microsoft.AspNetCore.Mvc;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Event;

namespace MovieRental.MVC.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ICookieService _cookieService;

        public EventsController(IEventService eventService, ICookieService cookieService)
        {
            _eventService = eventService;
            _cookieService = cookieService;
        }

        public async Task<IActionResult> Index(string? search, string? location, DateTime? fromDate, DateTime? toDate)
        {
            var currentLanguageId = await _cookieService.GetLanguageIdAsync();

            var filter = new EventFilterViewModel
            {
                CurrentLanguageId = currentLanguageId,
                SearchQuery = search,
                Location = location,
                FromDate = fromDate,
                ToDate = toDate
            };

            var viewModel = await _eventService.GetFilteredEventsAsync(filter);

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var currentLanguageId = await _cookieService.GetLanguageIdAsync();
            var eventEntity = await _eventService.GetEventByIdWithTranslationsAsync(id, currentLanguageId);

            if (eventEntity == null)
                return NotFound();

            return View(eventEntity);
        }
    }
}