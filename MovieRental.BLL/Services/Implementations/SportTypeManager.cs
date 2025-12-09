using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.SportType;
using MovieRental.DAL.DataContext.Entities;
using System.Linq.Expressions;

namespace MovieRental.BLL.Services.Implementations
{
    public class SportTypeManager : CrudManager<SportType, SportTypeViewModel, SportTypeCreateViewModel, SportTypeUpdateViewModel>, ISportTypeService
    {
        private readonly ICookieService _cookieService;

        public SportTypeManager(
            IRepositoryAsync<SportType> repository,
            IMapper mapper,
            ICookieService cookieService)
            : base(repository, mapper)
        {
            _cookieService = cookieService;
        }

        public override async Task<SportTypeViewModel?> GetByIdAsync(int id)
        {
            var languageId = await _cookieService.GetLanguageIdAsync();

            var sportType = await Repository.GetAsync(
                predicate: x => x.Id == id,
                include: query => query.Include(st => st.Translations.Where(t => t.LanguageId == languageId)),
                AsNoTracking: true
            );

            if (sportType == null) return null;

            var viewModel = Mapper.Map<SportTypeViewModel>(sportType);
            var translation = sportType.Translations?.FirstOrDefault();

            if (translation != null)
            {
                viewModel.Name = translation.Name;
            }

            return viewModel;
        }

        public override async Task<IEnumerable<SportTypeViewModel>> GetAllAsync(
            Expression<Func<SportType, bool>>? predicate = null,
            Func<IQueryable<SportType>, IOrderedQueryable<SportType>>? orderBy = null,
            Func<IQueryable<SportType>, IIncludableQueryable<SportType, object>>? include = null,
            bool AsNoTracking = false)
        {
            var languageId = await _cookieService.GetLanguageIdAsync();

            Func<IQueryable<SportType>, IIncludableQueryable<SportType, object>> includeWithTranslations = query =>
            {
                var included = query
                    .Include(st => st.Translations.Where(t => t.LanguageId == languageId))
                    .Include(st => st.Sports);

                return include != null ? include(included) : included;
            };

            var sportTypes = await Repository.GetAllAsync(predicate, orderBy, includeWithTranslations, AsNoTracking);

            return sportTypes.Select(st =>
            {
                var viewModel = Mapper.Map<SportTypeViewModel>(st);
                var translation = st.Translations?.FirstOrDefault();

                if (translation != null)
                {
                    viewModel.Name = translation.Name;
                }

                viewModel.SportCount = st.Sports?.Count(s => s.IsActive && s.EventDate > DateTime.Now) ?? 0;

                return viewModel;
            });
        }

        public async Task<IEnumerable<SportTypeViewModel>> GetActiveSportTypesAsync()
        {
            var languageId = await _cookieService.GetLanguageIdAsync();

            var sportTypes = await Repository.GetAllAsync(
                include: query => query
                    .Include(st => st.Translations.Where(t => t.LanguageId == languageId))
                    .Include(st => st.Sports.Where(s => s.IsActive && s.EventDate > DateTime.Now)),
                orderBy: query => query.OrderBy(st => st.Translations.FirstOrDefault()!.Name),
                AsNoTracking: true
            );

            return sportTypes.Select(st =>
            {
                var viewModel = Mapper.Map<SportTypeViewModel>(st);
                var translation = st.Translations?.FirstOrDefault();

                if (translation != null)
                {
                    viewModel.Name = translation.Name;
                }

                viewModel.SportCount = st.Sports?.Count ?? 0;

                return viewModel;
            });
        }

        public async Task<SportTypeViewModel?> GetSportTypeWithTranslationAsync(int id, int languageId)
        {
            var sportType = await Repository.GetAsync(
                predicate: x => x.Id == id,
                include: query => query.Include(st => st.Translations.Where(t => t.LanguageId == languageId)),
                AsNoTracking: true
            );

            if (sportType == null) return null;

            var viewModel = Mapper.Map<SportTypeViewModel>(sportType);
            var translation = sportType.Translations?.FirstOrDefault();

            if (translation != null)
            {
                viewModel.Name = translation.Name;
            }

            return viewModel;
        }

        public async Task<IEnumerable<SportTypeViewModel>> GetSportTypesWithCountAsync(int languageId)
        {
            var sportTypes = await Repository.GetAllAsync(
                include: query => query
                    .Include(st => st.Translations.Where(t => t.LanguageId == languageId))
                    .Include(st => st.Sports.Where(s => s.IsActive && s.EventDate > DateTime.Now)),
                orderBy: query => query.OrderBy(st => st.Translations.FirstOrDefault()!.Name),
                AsNoTracking: true
            );

            return sportTypes.Select(st =>
            {
                var viewModel = Mapper.Map<SportTypeViewModel>(st);
                var translation = st.Translations?.FirstOrDefault();

                if (translation != null)
                {
                    viewModel.Name = translation.Name;
                }

                viewModel.SportCount = st.Sports?.Count ?? 0;

                return viewModel;
            });
        }
    }
}
