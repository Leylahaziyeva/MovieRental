using Microsoft.AspNetCore.Mvc;
using MovieRental.BLL.Services.Contracts;

namespace MovieRental.MVC.Controllers
{
    public class OfferController : Controller
    {
        private readonly IOfferService _offerService;
        private readonly ICookieService _cookieService;

        public OfferController(IOfferService offerService, ICookieService cookieService)
        {
            _offerService = offerService;
            _cookieService = cookieService;
        }

        public async Task<IActionResult> Index()
        {
            var currentLanguageId = await _cookieService.GetLanguageIdAsync();
            var offers = await _offerService.GetActiveOffersAsync(currentLanguageId);

            return View(offers);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
                return BadRequest();

            var currentLanguageId = await _cookieService.GetLanguageIdAsync();
            var offer = await _offerService.GetOfferByIdWithTranslationsAsync(id, currentLanguageId);

            if (offer == null)
                return NotFound();

            if (!offer.IsValid)
            {
                TempData["Warning"] = "This offer has expired.";
            }

            return View(offer);
        }

        [HttpGet]
        public async Task<IActionResult> LoadMoreOffers(int page = 1, int pageSize = 8)
        {
            var currentLanguageId = await _cookieService.GetLanguageIdAsync();
            var allOffers = await _offerService.GetActiveOffersAsync(currentLanguageId);

            var pagedOffers = allOffers
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            if (!pagedOffers.Any())
                return Content(string.Empty);

            return PartialView("_OfferCardsPartial", pagedOffers); 
        }
    }
}