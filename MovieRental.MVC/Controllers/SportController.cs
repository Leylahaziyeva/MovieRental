using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.Services.Implementations;
using MovieRental.BLL.ViewModels.Sport;

namespace MovieRental.MVC.Controllers
{
    public class SportController : Controller
    {
        private readonly ISportService _sportService;
        private readonly IPersonService _personService;
        private readonly ICurrencyService _currencyService;
        private readonly ICookieService _cookieService;
        private readonly ISportTypeService _sportTypeService;
        private readonly ILocationService _locationService;

        public SportController(
            ISportService sportService,
            IPersonService personService,
            ICurrencyService currencyService,
            ICookieService cookieService,
            ISportTypeService sportTypeService,
            ILocationService locationService)
        {
            _sportService = sportService;
            _personService = personService;
            _currencyService = currencyService;
            _cookieService = cookieService;
            _sportTypeService = sportTypeService;
            _locationService = locationService;
        }

        public async Task<IActionResult> Index(int page = 1, int pageSize = 12,
    List<int>? sportTypeIds = null, List<int>? locationIds = null,
    DateTime? startDate = null, DateTime? endDate = null)
        {
            var languageId = await _cookieService.GetLanguageIdAsync();

            // POPULATE FILTER OPTIONS
            var filterOptions = await _sportService.GetFilterOptionsAsync(languageId);

            var filter = new SportFilterViewModel
            {
                Page = page,
                PageSize = pageSize,
                SportTypeIds = sportTypeIds ?? new List<int>(),
                LocationIds = locationIds ?? new List<int>(),
                StartDate = startDate,
                EndDate = endDate,
                SportTypes = filterOptions.SportTypes,
                Locations = filterOptions.Locations
            };

            var result = await _sportService.GetFilteredSportsAsync(filter);

            // SET VIEWBAG FOR PARTIAL
            ViewBag.FilterType = "Sport";
            ViewBag.ActionName = "Index";
            ViewBag.Filter = result.Filter;
            ViewBag.SportTypes = result.Filter.SportTypes;
            ViewBag.Locations = result.Filter.Locations;

            return View(result.Sports);
        }

        //public async Task<IActionResult> Index(int page = 1, int pageSize = 12, List<int>? sportTypeIds = null, List<int>? locationIds = null, DateTime? startDate = null, DateTime? endDate = null)
        //{
        //    var languageId = await _cookieService.GetLanguageIdAsync();

        //    var filterOptions = await _sportService.GetFilterOptionsAsync(languageId);

        //    var filter = new SportFilterViewModel
        //    {
        //        Page = page,
        //        PageSize = pageSize,
        //        SportTypeIds = sportTypeIds ?? new List<int>(),
        //        LocationIds = locationIds ?? new List<int>(),
        //        StartDate = startDate,
        //        EndDate = endDate,
        //        SportTypes = filterOptions.SportTypes,
        //        Locations = filterOptions.Locations
        //    };

        //    var result = await _sportService.GetFilteredSportsAsync(filter);

        //    ViewBag.FilterType = "Sport";
        //    ViewBag.ActionName = "Index";
        //    ViewBag.Filter = result.Filter;
        //    ViewBag.SportTypes = result.Filter.SportTypes;
        //    ViewBag.Locations = result.Filter.Locations;

        //    return View(result.Sports);
        //}
        public async Task<IActionResult> Details(int id)
        {
            var sport = await _sportService.GetSportDetailAsync(id);

            if (sport == null)
            {
                return NotFound();
            }

            return View(sport);
        }

        public async Task<IActionResult> Create()
        {
            var languageId = await _cookieService.GetLanguageIdAsync();

            var currencies = await _currencyService.GetAllAsync();
            var sportTypes = await _sportTypeService.GetActiveSportTypesAsync();
            var locations = await _locationService.GetLocationsForFilterAsync(languageId);

            var model = new SportCreateViewModel
            {
                Name = string.Empty,
                Description = string.Empty,
                Location = string.Empty,
                ImageFile = null!,
                EventDate = DateTime.Now.AddDays(7),
                SportTypeList = sportTypes.Select(st => new SelectListItem
                {
                    Value = st.Id.ToString(),
                    Text = st.Name ?? "Unknown"
                }).ToList(),
                LocationList = locations.Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Name
                }).ToList(),
                CurrencyList = currencies.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Name} ({c.Symbol})"
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(SportCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _sportService.CreateAsync(model);
                TempData["Success"] = "Sport event successfully created!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating sport: {ex.Message}");
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var sport = await _sportService.GetByIdAsync(id);

            if (sport == null)
            {
                return NotFound();
            }

            var languageId = await _cookieService.GetLanguageIdAsync();

            var currencies = await _currencyService.GetAllAsync();
            var sportTypes = await _sportTypeService.GetActiveSportTypesAsync();
            var locations = await _locationService.GetLocationsForFilterAsync(languageId);

            var model = new SportUpdateViewModel
            {
                Id = sport.Id,
                Name = sport.Name,
                Description = sport.Description,
                Location = sport.Location,
                ImageUrl = sport.ImageUrl,
                CoverImageUrl = sport.CoverImageUrl,
                EventDate = sport.EventDate,
                Price = sport.Price,
                IsActive = sport.IsActive,
                IsFeatured = sport.IsFeatured,
                ContactPhone = sport.ContactPhone,
                ContactEmail = sport.ContactEmail,
                Venue = sport.Venue,
                GoogleMapsUrl = sport.GoogleMapsUrl,
                AgeRestriction = sport.AgeRestriction,
                CurrencyId = sport.Currency?.Id,
                SportTypeList = sportTypes.Select(st => new SelectListItem
                {
                    Value = st.Id.ToString(),
                    Text = st.Name ?? "Unknown"
                }).ToList(),
                LocationList = locations.Select(l => new SelectListItem
                {
                    Value = l.Id.ToString(),
                    Text = l.Name
                }).ToList(),
                CurrencyList = currencies.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Name} ({c.Symbol})",
                    Selected = c.Id == sport.Currency?.Id
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, SportUpdateViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _sportService.UpdateAsync(id, model);

                if (!result)
                {
                    return NotFound();
                }

                TempData["Success"] = "Sport event successfully updated!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating sport: {ex.Message}");
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _sportService.DeleteAsync(id);

                if (!result)
                {
                    TempData["Error"] = "Sport event not found!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Success"] = "Sport event successfully deleted!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting sport: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> GetUpcoming()
        {
            var sports = await _sportService.GetUpcomingSportsAsync();
            return Json(sports);
        }

        public async Task<IActionResult> GetFeatured(int count = 4)
        {
            var sports = await _sportService.GetFeaturedSportsAsync(count);
            return Json(sports);
        }

        [HttpGet]
        public async Task<IActionResult> LoadMoreSports(
    int page = 1,
    int pageSize = 12,
    List<int>? sportTypeIds = null,
    List<int>? locationIds = null,
    DateTime? startDate = null,
    DateTime? endDate = null)
        {
            var languageId = await _cookieService.GetLanguageIdAsync();
            var filterOptions = await _sportService.GetFilterOptionsAsync(languageId);

            var filter = new SportFilterViewModel
            {
                Page = page,
                PageSize = pageSize,
                SportTypeIds = sportTypeIds ?? new List<int>(),
                LocationIds = locationIds ?? new List<int>(),
                StartDate = startDate,
                EndDate = endDate,
                SportTypes = filterOptions.SportTypes,
                Locations = filterOptions.Locations
            };

            var result = await _sportService.GetFilteredSportsAsync(filter);

            if (!result.Sports.Any())
                return Content(string.Empty);

            return PartialView("_SportCardsPartial", result.Sports);
        }
    }
}