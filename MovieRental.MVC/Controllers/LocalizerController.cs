using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using MovieRental.BLL.Services.Contracts;

namespace MovieRental.MVC.Controllers
{
    public class LocalizerController : Controller
    {
        private readonly ILanguageService _languageService;

        public LocalizerController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        public IActionResult ChangeCulture(string culture)
        {
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions() { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public IActionResult ChangeCurrency(string code)
        {
            Response.Cookies.Append("currency", code,
               new CookieOptions() { Expires = DateTimeOffset.UtcNow.AddYears(1) });

            return Redirect(Request.Headers["Referer"].ToString());
        }

        public string GetCurrencyCode()
        {
            var culture = Request.Cookies["currency"];

            return culture ?? "azn";
        }

        public async Task<int> GetLanguageAsync()
        {
            var culture = Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];
            var isoCode = culture?.Substring(culture.LastIndexOf("=") + 1) ?? "en-Us";
            var selectedLanguage = await _languageService.GetAsync(x => x.IsoCode == isoCode);

            return selectedLanguage!.Id;
        }
    }
}