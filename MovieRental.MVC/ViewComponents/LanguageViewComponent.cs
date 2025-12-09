using Microsoft.AspNetCore.Mvc;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Header;

namespace MovieRental.MVC.ViewComponents
{
    public class LanguageViewComponent : ViewComponent
    {
        private readonly ILanguageService _languageService;
        private readonly ICookieService _cookieService;

        public LanguageViewComponent(ILanguageService languageService, ICookieService cookieService)
        {
            _languageService = languageService;
            _cookieService = cookieService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var selectedLanguage = await _cookieService.GetLanguageAsync();
            var currentLanguageId = selectedLanguage?.Id ?? 1; 

            var languages = await _languageService.GetAllWithTranslationsAsync(currentLanguageId);

            var headerViewModel = new HeaderViewModel
            {
                Languages = languages.ToList(),
                SelectedLanguage = selectedLanguage
            };

            return View(headerViewModel);
        }
    }
}
