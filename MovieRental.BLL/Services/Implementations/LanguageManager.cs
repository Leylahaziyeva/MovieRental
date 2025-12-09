using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Language;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Implementations
{
    public class LanguageManager : CrudManager<Language, LanguageViewModel, LanguageCreateViewModel, LanguageUpdateViewModel>, ILanguageService
    {
        private readonly IRepositoryAsync<Language> _repository;
        private readonly IMapper _mapper;

        public LanguageManager(IRepositoryAsync<Language> repository, IMapper mapper) : base(repository, mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<LanguageViewModel>> GetAllWithTranslationsAsync(int? translationLanguageId = null)
        {
            Func<IQueryable<Language>, IIncludableQueryable<Language, object>>? include = query =>
            {
                if (translationLanguageId.HasValue)
                {
                    return query.Include(l => l.LanguageTranslations
                        .Where(lt => lt.TranslationLanguageId == translationLanguageId.Value));
                }
                return query.Include(l => l.LanguageTranslations);
            };

            return await GetAllAsync(
                predicate: l => !l.IsDeleted,
                include: include,
                AsNoTracking: true
            );
        }
    }
}