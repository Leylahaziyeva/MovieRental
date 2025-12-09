using Microsoft.AspNetCore.Mvc;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Header;

namespace MovieRental.MVC.ViewComponents
{
    public class CurrencyViewComponent : ViewComponent
    {
        private readonly ICurrencyService _currencyService;
        private readonly ICookieService _cookieService;

        public CurrencyViewComponent(ICurrencyService currencyService, ICookieService cookieService)
        {
            _currencyService = currencyService;
            _cookieService = cookieService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var selectedCurrency = await _cookieService.GetCurrencyAsync();
            var selectedLanguage = await _cookieService.GetLanguageAsync();
            var currentLanguageId = selectedLanguage?.Id ?? 1;

            var currencies = await _currencyService.GetAllWithTranslationsAsync(currentLanguageId);

            var headerViewModel = new HeaderViewModel
            {
                Currencies = currencies.ToList(),
                SelectedCurrency = selectedCurrency
            };

            return View(headerViewModel);
        }
    }
}
