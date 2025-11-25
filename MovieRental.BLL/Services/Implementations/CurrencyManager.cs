using AutoMapper;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Currency;
using MovieRental.DAL.DataContext.Entities;
using MovieRental.DAL.Repositories.Contracts;

namespace MovieRental.BLL.Services.Implementations
{
    public class CurrencyManager : CrudManager<Currency, CurrencyViewModel, CurrencyCreateViewModel, CurrencyUpdateViewModel>, ICurrencyService
    {
        private readonly IRepositoryAsync<Currency> _currencyRepository; 
        public CurrencyManager(IRepositoryAsync<Currency> currencyRepository, IMapper mapper) : base(currencyRepository, mapper) 
        {
            _currencyRepository = currencyRepository;
        }
    }
}
