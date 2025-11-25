using MovieRental.BLL.ViewModels.Currency;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Contracts
{
    public interface ICurrencyService : ICrudService<Currency, CurrencyViewModel,  CurrencyCreateViewModel, CurrencyUpdateViewModel>
    {
    }
}