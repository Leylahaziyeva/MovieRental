using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MovieRental.DAL.DataContext.Entities;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Account;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HeaderManager(
            AppDbContext dbContext,
            ILanguageService languageService,
            ICurrencyService currencyService,
            ICookieService cookieService,
            UserManager<AppUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _languageService = languageService;
            _currencyService = currencyService;
            _cookieService = cookieService;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<HeaderViewModel> GetHeaderAsync()
        {
            var languages = await _languageService.GetAllAsync();
            var selectedLanguage = await _cookieService.GetLanguageAsync();
            var currencies = await _currencyService.GetAllAsync();
            var selectedCurrency = await _cookieService.GetCurrencyAsync();

            AccountViewModel? accountViewModel = null;
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated == true)
            {
                var appUser = await _userManager.GetUserAsync(user);
                if (appUser != null)
                {
                    accountViewModel = new AccountViewModel
                    {
                        UserName = appUser.UserName ?? "",
                        FirstName = appUser.FirstName ?? "",
                        LastName = appUser.LastName ?? "",
                        Email = appUser.Email ?? "",
                        ProfileImagePath = appUser.ProfileImage,
                        Company = appUser.Company,
                        Address = appUser.Address,
                        PhoneNumber = appUser.PhoneNumber
                    };
                }
            }

            var headerViewModel = new HeaderViewModel
            {
                Languages = languages.ToList(),
                SelectedLanguage = selectedLanguage,
                Currencies = currencies.ToList(),
                SelectedCurrency = selectedCurrency,
                Account = accountViewModel 
            };

            return headerViewModel;
        }
    }
}