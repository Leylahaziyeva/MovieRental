using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Currency;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Implementations
{
    public class CurrencyManager : CrudManager<Currency, CurrencyViewModel, CurrencyCreateViewModel, CurrencyUpdateViewModel>, ICurrencyService
    {
        private readonly IRepositoryAsync<Currency> _repository;
        private readonly IMapper _mapper;

        public CurrencyManager(IRepositoryAsync<Currency> repository, IMapper mapper) : base(repository, mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CurrencyViewModel>> GetAllWithTranslationsAsync(int? translationLanguageId = null)
        {
            Func<IQueryable<Currency>, IIncludableQueryable<Currency, object>>? include = query =>
            {
                if (translationLanguageId.HasValue)
                {
                    return query.Include(c => c.Translations
                        .Where(ct => ct.LanguageId == translationLanguageId.Value));
                }
                return query.Include(c => c.Translations);
            };

            return await GetAllAsync(
                predicate: c => !c.IsDeleted,
                include: include,
                AsNoTracking: true
            );
        }
    }
}