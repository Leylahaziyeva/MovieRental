using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Currency;
using MovieRental.BLL.ViewModels.Language;

namespace MovieRental.BLL.Services.Implementations
{
    public class CookieManager : ICookieService
    {
        private const string BROWSER_ID_KEY = "browserId";

        private readonly ILanguageService _languageService;
        private readonly ICurrencyService _currencyService;
        private readonly IHttpContextAccessor _contextAccessor;

        public CookieManager(ILanguageService languageService, ICurrencyService currencyService, IHttpContextAccessor contextAccessor)
        {
            _languageService = languageService;
            _currencyService = currencyService;
            _contextAccessor = contextAccessor;
        }

        public void AddBrowserId()
        {
            var existBrowserId = _contextAccessor.HttpContext.Request.Cookies[BROWSER_ID_KEY];

            if (!string.IsNullOrEmpty(existBrowserId)) return;

            _contextAccessor.HttpContext.Response.Cookies.Append(BROWSER_ID_KEY, Guid.NewGuid().ToString());
        }

        public string GetBrowserId()
        {
            var existBrowserId = _contextAccessor.HttpContext.Request.Cookies[BROWSER_ID_KEY];

            if (!string.IsNullOrEmpty(existBrowserId)) return existBrowserId;

            var newGuid = Guid.NewGuid().ToString();
            _contextAccessor.HttpContext.Response.Cookies.Append(BROWSER_ID_KEY, newGuid);

            return newGuid;
        }

        public async Task<CurrencyViewModel> GetCurrencyAsync()
        {
            var currencies = await _currencyService.GetAllAsync();
            var currencyIsoCode = _contextAccessor.HttpContext.Request.Cookies["currency"] ?? "az-az";
            var selectedCurrency = await _currencyService.GetAsync(x => x.IsoCode == currencyIsoCode);

            return selectedCurrency;
        }

        public async Task<LanguageViewModel> GetLanguageAsync()
        {
            var languages = await _languageService.GetAllAsync();
            var culture = _contextAccessor.HttpContext.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];
            var isoCode = culture?.Substring(culture.LastIndexOf("=") + 1) ?? "en-Us";
            var selectedLanguage = await _languageService.GetAsync(x => x.IsoCode == isoCode);

            return selectedLanguage;
        }

        public async Task<int> GetLanguageIdAsync()
        {
            var language = await GetLanguageAsync();
            return language?.Id ?? 1; 
        }

    }
}