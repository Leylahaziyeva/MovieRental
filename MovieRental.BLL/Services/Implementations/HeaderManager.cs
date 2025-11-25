using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Header;
using MovieRental.DAL.DataContext;

namespace MovieRental.BLL.Services.Implementations
{
    public class HeaderManager : IHeaderService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILanguageService _languageService;
        private readonly ICurrencyService _currencyService;
        private readonly ICookieService _cookieService;

        public HeaderManager(
            AppDbContext dbContext,
            ILanguageService languageService,
            ICurrencyService currencyService,
            ICookieService cookieService)
        {
            _dbContext = dbContext;
            _languageService = languageService;
            _currencyService = currencyService;
            _cookieService = cookieService;
        }

        public async Task<HeaderViewModel> GetHeaderAsync()
        {
            var languages = await _languageService.GetAllAsync();
            var selectedLanguage = await _cookieService.GetLanguageAsync();
            var currencies = await _currencyService.GetAllAsync();
            var selectedCurrency = await _cookieService.GetCurrencyAsync();

            var headerViewModel = new HeaderViewModel
            {
                Languages = languages.ToList(),
                SelectedLanguage = selectedLanguage,
                Currencies = currencies.ToList(),
                SelectedCurrency = selectedCurrency 

                // Digər header məlumatları
                //Logo = await _dbContext.Logos
                //    .OrderByDescending(x => x.CreatedAt)
                //    .FirstOrDefaultAsync(),

                //SearchInfo = await _dbContext.SearchInfos
                //    .OrderByDescending(x => x.CreatedAt)
                //    .FirstOrDefaultAsync() ?? new SearchInfo()
            };

            return headerViewModel;
        }
    }
}