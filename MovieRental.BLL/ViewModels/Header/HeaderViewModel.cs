using MovieRental.BLL.ViewModels.Account;
using MovieRental.BLL.ViewModels.Currency;
using MovieRental.BLL.ViewModels.Language;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.ViewModels.Header
{
    public class HeaderViewModel
    {
        public Logo Logo { get; set; } = null!;
        public List<LanguageViewModel>? Languages { get; set; }
        public LanguageViewModel? SelectedLanguage { get; set; }     
        public List<CurrencyViewModel> Currencies { get; set; } = [];
        public CurrencyViewModel? SelectedCurrency { get; set; }
        public AccountViewModel? Account { get; set; }
        //public SearchInfo? SearchInfo { get; set; }
    }
}
