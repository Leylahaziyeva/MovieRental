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
            var currentLanguageId = await _cookieService.GetLanguageIdAsync();
            var offer = await _offerService.GetOfferByIdWithTranslationsAsync(id, currentLanguageId);

            if (offer == null)
                return NotFound();

            return View(offer);
        }
    }
}