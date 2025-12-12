using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.Services.Implementations;
using MovieRental.BLL.ViewModels.Event;
using MovieRental.BLL.ViewModels.EventCategory;
using MovieRental.BLL.ViewModels.Location;

namespace MovieRental.MVC.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ICookieService _cookieService;
        private readonly ICurrencyService _currencyService;
        private readonly IPersonService _personService;
        private readonly ILocationService _locationService;
        private readonly IEventCategoryService _eventCategoryService;

        public EventController(
            IEventService eventService,
            ICookieService cookieService,
            ICurrencyService currencyService,
            IPersonService personService,
            ILocationService locationService,
            IEventCategoryService eventCategoryService)
        {
            _eventService = eventService;
            _cookieService = cookieService;
            _currencyService = currencyService;
            _personService = personService;
            _locationService = locationService;
            _eventCategoryService = eventCategoryService;
        }

        #region Details with Artists

        public async Task<IActionResult> Details(int id)
        {
            var currentLanguageId = await _cookieService.GetLanguageIdAsync();
            var eventEntity = await _eventService.GetEventByIdWithTranslationsAsync(id, currentLanguageId);

            if (eventEntity == null)
                return NotFound();

            // Debug: Check if artists are loaded
            Console.WriteLine($"Event ID: {id}");
            Console.WriteLine($"Artists Count: {eventEntity.Artists?.Count ?? 0}");
            if (eventEntity.Artists != null)
            {
                foreach (var artist in eventEntity.Artists)
                {
                    Console.WriteLine($"Artist: {artist.Name} (ID: {artist.Id})");
                }
            }

            return View(eventEntity);
        }

        #endregion

        #region Index
        public async Task<IActionResult> Index(EventFilterViewModel filter)
        {
            var currentLanguageId = await _cookieService.GetLanguageIdAsync();
            filter.CurrentLanguageId = currentLanguageId;

            var viewModel = await _eventService.GetFilteredEventsAsync(filter);

            return View(viewModel);
        }

        #endregion

        #region Create

        public async Task<IActionResult> Create()
        {
            var currentLanguageId = await _cookieService.GetLanguageIdAsync();
            var currencies = await _currencyService.GetAllAsync();
            var artists = await _personService.GetArtistsAsync();
            var categories = await _eventCategoryService.GetCategoriesForFilterAsync(currentLanguageId);
            var locations = await _locationService.GetLocationsForFilterAsync(currentLanguageId);

            var model = new EventCreateViewModel
            {
                Name = string.Empty,
                Description = string.Empty,
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
                }).ToList(),
                EventCategoryList = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                }).ToList(),
                LocationList = locations.Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Name
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EventCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCreateDropdownsAsync(model);
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
                await PopulateCreateDropdownsAsync(model);
                return View(model);
            }
        }

        #endregion

        #region Edit

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
            var categories = await _eventCategoryService.GetCategoriesForFilterAsync(currentLanguageId);
            var locations = await _locationService.GetLocationsForFilterAsync(currentLanguageId);

            var model = new EventUpdateViewModel
            {
                Id = eventEntity.Id,
                Name = eventEntity.Name,
                Description = eventEntity.Description,
                ImageUrl = eventEntity.ImageUrl,
                CoverImageUrl = eventEntity.CoverImageUrl,
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
                EventCategoryId = GetCategoryIdFromName(eventEntity.CategoryName, categories),
                LocationId = GetLocationIdFromName(eventEntity.LocationName, locations),
                SelectedArtistIds = eventEntity.Artists?.Select(a => a.Id).ToList() ?? new List<int>(),
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
                }).ToList(),
                EventCategoryList = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name,
                    Selected = c.Name == eventEntity.CategoryName
                }).ToList(),
                LocationList = locations.Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Name,
                    Selected = l.Name == eventEntity.LocationName
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EventUpdateViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await PopulateUpdateDropdownsAsync(model);
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
                await PopulateUpdateDropdownsAsync(model);
                return View(model);
            }
        }

        #endregion

        #region Delete

        [HttpPost]
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

        #endregion

        #region Artist Management - FIXED

        [HttpPost]
        public async Task<IActionResult> AddArtists(int eventId, List<int> artistIds)
        {
            try
            {
                if (artistIds == null || !artistIds.Any())
                {
                    TempData["Error"] = "Please select at least one artist!";
                    return RedirectToAction(nameof(Details), new { id = eventId });
                }

                var result = await _eventService.AddArtistsToEventAsync(eventId, artistIds);

                if (!result)
                {
                    TempData["Error"] = "Failed to add artists!";
                }
                else
                {
                    TempData["Success"] = $"{artistIds.Count} artist(s) successfully added!";
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

        // NEW: Manage Artists Page
        public async Task<IActionResult> ManageArtists(int id)
        {
            var currentLanguageId = await _cookieService.GetLanguageIdAsync();
            var eventEntity = await _eventService.GetEventByIdWithTranslationsAsync(id, currentLanguageId);

            if (eventEntity == null)
            {
                return NotFound();
            }

            var allArtists = await _personService.GetArtistsAsync();
            var currentArtistIds = eventEntity.Artists?.Select(a => a.Id).ToList() ?? new List<int>();

            ViewBag.EventId = id;
            ViewBag.EventName = eventEntity.Name;
            ViewBag.CurrentArtistIds = currentArtistIds;
            ViewBag.AllArtists = allArtists;

            return View(eventEntity);
        }

        #endregion

        #region API Methods

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

        #endregion

        #region Helper Methods

        private async Task PopulateCreateDropdownsAsync(EventCreateViewModel model)
        {
            var currentLanguageId = await _cookieService.GetLanguageIdAsync();
            var currencies = await _currencyService.GetAllAsync();
            var artists = await _personService.GetArtistsAsync();
            var categories = await _eventCategoryService.GetCategoriesForFilterAsync(currentLanguageId);
            var locations = await _locationService.GetLocationsForFilterAsync(currentLanguageId);

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

            model.EventCategoryList = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            model.LocationList = locations.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = l.Name
            }).ToList();
        }

        private async Task PopulateUpdateDropdownsAsync(EventUpdateViewModel model)
        {
            var currentLanguageId = await _cookieService.GetLanguageIdAsync();
            var currencies = await _currencyService.GetAllAsync();
            var artists = await _personService.GetArtistsAsync();
            var categories = await _eventCategoryService.GetCategoriesForFilterAsync(currentLanguageId);
            var locations = await _locationService.GetLocationsForFilterAsync(currentLanguageId);

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

            model.EventCategoryList = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();

            model.LocationList = locations.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = l.Name
            }).ToList();
        }

        private int? GetCategoryIdFromName(string? categoryName, IEnumerable<EventCategoryOption> categories)
        {
            if (string.IsNullOrEmpty(categoryName)) return null;
            return categories.FirstOrDefault(c => c.Name == categoryName)?.Id;
        }

        private int? GetLocationIdFromName(string? locationName, IEnumerable<LocationOption> locations)
        {
            if (string.IsNullOrEmpty(locationName)) return null;
            return locations.FirstOrDefault(l => l.Name == locationName)?.Id;
        }

        #endregion
    }
}