using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Sport;

namespace MovieRental.MVC.Controllers
{
    public class SportController : Controller
    {
        private readonly ISportService _sportService;
        private readonly IPersonService _personService;
        private readonly ICurrencyService _currencyService;

        public SportController(ISportService sportService, IPersonService personService, ICurrencyService currencyService)
        {
            _sportService = sportService;
            _personService = personService;
            _currencyService = currencyService;
        }
        public async Task<IActionResult> Index(string? location, string? category, string? language, DateTime? startDate, DateTime? endDate)
        {
            IEnumerable<SportViewModel> sports;

            if (!string.IsNullOrEmpty(location))
            {
                sports = await _sportService.GetSportsByLocationAsync(location);
            }
            else if (!string.IsNullOrEmpty(category))
            {
                sports = await _sportService.GetSportsByCategoryAsync(category);
            }
            else if (startDate.HasValue && endDate.HasValue)
            {
                sports = await _sportService.GetSportsByDateRangeAsync(startDate.Value, endDate.Value);
            }
            else
            {
                sports = await _sportService.GetUpcomingSportsAsync();
            }

            ViewData["Location"] = location;
            ViewData["Category"] = category;
            ViewData["Language"] = language;
            ViewData["StartDate"] = startDate;
            ViewData["EndDate"] = endDate;

            return View(sports);
        }

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
            var currencies = await _currencyService.GetAllAsync();

            var model = new SportCreateViewModel
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
                }).ToList()
            };

            return View(model);
        }

        /// Sport yaratma (Admin)
        [HttpPost]
        //[ValidateAntiForgeryToken]
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

        /// GET: /Sport/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var sport = await _sportService.GetByIdAsync(id);

            if (sport == null)
            {
                return NotFound();
            }

            var currencies = await _currencyService.GetAllAsync();

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
                Categories = sport.Categories,
                Languages = sport.Languages,
                AgeRestriction = sport.AgeRestriction,
                CurrencyId = sport.Currency?.Id,
                CurrencyList = currencies.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.Name} ({c.Symbol})",
                    Selected = c.Id == sport.Currency?.Id
                }).ToList()
            };

            return View(model);
        }

        /// Sport redaktə (Admin)
        [HttpPost]
        //[ValidateAntiForgeryToken]
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

        /// Sport silmə (Admin)
        [HttpPost]
        //[ValidateAntiForgeryToken]
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

        /// Player əlavə et (Admin)
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPlayers(int sportId, List<int> playerIds)
        {
            try
            {
                var result = await _sportService.AddPlayersToSportAsync(sportId, playerIds);

                if (!result)
                {
                    TempData["Error"] = "Failed to add players!";
                }
                else
                {
                    TempData["Success"] = "Players successfully added!";
                }

                return RedirectToAction(nameof(Details), new { id = sportId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = sportId });
            }
        }

        /// Player sil (Admin)
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> RemovePlayer(int sportId, int playerId)
        {
            try
            {
                var result = await _sportService.RemovePlayerFromSportAsync(sportId, playerId);

                if (!result)
                {
                    TempData["Error"] = "Failed to remove player!";
                }
                else
                {
                    TempData["Success"] = "Player successfully removed!";
                }

                return RedirectToAction(nameof(Details), new { id = sportId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id = sportId });
            }
        }

        /// GET: /Sport/GetUpcoming
        public async Task<IActionResult> GetUpcoming()
        {
            var sports = await _sportService.GetUpcomingSportsAsync();
            return Json(sports);
        }

        /// GET: /Sport/GetFeatured
        public async Task<IActionResult> GetFeatured(int count = 4)
        {
            var sports = await _sportService.GetFeaturedSportsAsync(count);
            return Json(sports);
        }

        [HttpGet]
        public async Task<IActionResult> LoadMoreSports(int page = 1, int pageSize = 12, string? location = null, string? category = null)
        {
            var (sports, totalCount) = await _sportService.GetSportsPagedAsync(page, pageSize, location, category);

            if (!sports.Any())
                return Content(string.Empty);

            return PartialView("_SportCardsPartial", sports); 
        }
    }
}