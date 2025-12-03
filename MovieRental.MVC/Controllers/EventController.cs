using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Event;

namespace MovieRental.MVC.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ICookieService _cookieService;
        private readonly ICurrencyService _currencyService;
        private readonly IPersonService _personService;

        public EventController(
            IEventService eventService,
            ICookieService cookieService,
            ICurrencyService currencyService,
            IPersonService personService)
        {
            _eventService = eventService;
            _cookieService = cookieService;
            _currencyService = currencyService;
            _personService = personService;
        }

        public async Task<IActionResult> Index(EventFilterViewModel filter)
        {
            var currentLanguageId = await _cookieService.GetLanguageIdAsync();
            filter.CurrentLanguageId = currentLanguageId;

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

        public async Task<IActionResult> Create()
        {
            var currencies = await _currencyService.GetAllAsync();
            var artists = await _personService.GetArtistsAsync();

            var model = new EventCreateViewModel
            {
                Name = string.Empty,
                Description = string.Empty,
                Location = string.Empty,
                ImageFile = null!,
                EventDate = DateTime.Now.AddDays(7),
                CurrencyList = currencies.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Name} ({c.Symbol})"
                }).ToList(),
                ArtistList = artists.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var currencies = await _currencyService.GetAllAsync();
                var artists = await _personService.GetArtistsAsync();

                model.CurrencyList = currencies.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Name} ({c.Symbol})"
                }).ToList();

                model.ArtistList = artists.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }).ToList();

                return View(model);
            }

            try
            {
                await _eventService.CreateAsync(model);
                TempData["Success"] = "Event successfully created!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating event: {ex.Message}");

                var currencies = await _currencyService.GetAllAsync();
                var artists = await _personService.GetArtistsAsync();

                model.CurrencyList = currencies.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Name} ({c.Symbol})"
                }).ToList();

                model.ArtistList = artists.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }).ToList();

                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var currentLanguageId = await _cookieService.GetLanguageIdAsync();
            var eventEntity = await _eventService.GetEventByIdWithTranslationsAsync(id, currentLanguageId);

            if (eventEntity == null)
            {
                return NotFound();
            }

            var currencies = await _currencyService.GetAllAsync();
            var artists = await _personService.GetArtistsAsync();

            var model = new EventUpdateViewModel
            {
                Id = eventEntity.Id,
                Name = eventEntity.Name,
                Description = eventEntity.Description,
                Location = eventEntity.Location,
                ImageUrl = eventEntity.ImageUrl,
                CoverImageUrl = eventEntity.CoverImageUrl,
                Categories = eventEntity.Categories,
                Languages = eventEntity.Languages,
                EventDate = eventEntity.EventDate,
                Price = eventEntity.Price,
                IsActive = eventEntity.IsActive,
                IsFeatured = eventEntity.IsFeatured,
                ContactPhone = eventEntity.ContactPhone,
                ContactEmail = eventEntity.ContactEmail,
                Venue = eventEntity.Venue,
                GoogleMapsUrl = eventEntity.GoogleMapsUrl,
                AgeRestriction = eventEntity.AgeRestriction,
                CurrencyId = eventEntity.Currency?.Id,
                SelectedArtistIds = eventEntity.Artists?.Select(a => a.Id).ToList(),
                CurrencyList = currencies.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Name} ({c.Symbol})",
                    Selected = c.Id == eventEntity.Currency?.Id
                }).ToList(),
                ArtistList = artists.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name,
                    Selected = eventEntity.Artists?.Any(art => art.Id == a.Id) ?? false
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EventUpdateViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                var currencies = await _currencyService.GetAllAsync();
                var artists = await _personService.GetArtistsAsync();

                model.CurrencyList = currencies.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Name} ({c.Symbol})"
                }).ToList();

                model.ArtistList = artists.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }).ToList();

                return View(model);
            }

            try
            {
                var result = await _eventService.UpdateAsync(id, model);

                if (!result)
                {
                    return NotFound();
                }

                TempData["Success"] = "Event successfully updated!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating event: {ex.Message}");

                var currencies = await _currencyService.GetAllAsync();
                var artists = await _personService.GetArtistsAsync();

                model.CurrencyList = currencies.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Name} ({c.Symbol})"
                }).ToList();

                model.ArtistList = artists.Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = a.Name
                }).ToList();

                return View(model);
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _eventService.DeleteAsync(id);

                if (!result)
                {
                    TempData["Error"] = "Event not found!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Success"] = "Event successfully deleted!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting event: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> AddArtists(int eventId, List<int> artistIds)
        {
            try
            {
                var result = await _eventService.AddArtistsToEventAsync(eventId, artistIds);

                if (!result)
                {
                    TempData["Error"] = "Failed to add artists!";
                }
                else
                {
                    TempData["Success"] = "Artists successfully added!";
                }

                return RedirectToAction(nameof(Details), new { id = eventId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = eventId });
            }
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveArtist(int eventId, int artistId)
        {
            try
            {
                var result = await _eventService.RemoveArtistFromEventAsync(eventId, artistId);

                if (!result)
                {
                    TempData["Error"] = "Failed to remove artist!";
                }
                else
                {
                    TempData["Success"] = "Artist successfully removed!";
                }

                return RedirectToAction(nameof(Details), new { id = eventId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = eventId });
            }
        }

        public async Task<IActionResult> GetUpcoming()
        {
            var currentLanguageId = await _cookieService.GetLanguageIdAsync();
            var events = await _eventService.GetUpcomingEventsAsync(currentLanguageId);
            return Json(events);
        }

        public async Task<IActionResult> GetFeatured(int count = 4)
        {
            var currentLanguageId = await _cookieService.GetLanguageIdAsync();
            var events = await _eventService.GetUpcomingEventsAsync(currentLanguageId);
            var featured = events.Where(e => e.IsFeatured).Take(count);
            return Json(featured);
        }

        [HttpGet]
        public async Task<IActionResult> LoadMoreEvents([FromQuery] EventFilterViewModel filter)
        {
            filter.CurrentLanguageId = await _cookieService.GetLanguageIdAsync();
            var (events, totalCount) = await _eventService.GetEventsPagedAsync(filter);

            if (!events.Any())
                return Content(string.Empty);

            return PartialView("_EventCardsPartial", events); 
        }
    }
}