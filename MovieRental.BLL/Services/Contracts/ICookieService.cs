using MovieRental.BLL.ViewModels.Currency;
using MovieRental.BLL.ViewModels.Language;

namespace MovieRental.BLL.Services.Contracts
{
    public interface ICookieService
    {
        Task<LanguageViewModel> GetLanguageAsync();
        Task<int> GetLanguageIdAsync();
        Task<CurrencyViewModel> GetCurrencyAsync();
        void AddBrowserId();
        string GetBrowserId();
    }
}