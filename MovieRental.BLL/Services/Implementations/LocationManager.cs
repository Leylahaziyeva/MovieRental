using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Location;
using MovieRental.DAL.DataContext.Entities;
using System.Linq.Expressions;

namespace MovieRental.BLL.Services.Implementations
{
    public class LocationManager : CrudManager<Location, LocationViewModel, LocationCreateViewModel, LocationUpdateViewModel>, ILocationService
    {
        private readonly ICookieService _cookieService;

        public LocationManager(
            IRepositoryAsync<Location> repository,
            IMapper mapper,
            ICookieService cookieService)
            : base(repository, mapper)
        {
            _cookieService = cookieService;
        }

        public override async Task<LocationViewModel?> GetByIdAsync(int id)
        {
            var languageId = await _cookieService.GetLanguageIdAsync();

            var location = await Repository.GetAsync(
                predicate: x => x.Id == id,
                include: query => query.Include(l => l.Translations.Where(t => t.LanguageId == languageId)),
                AsNoTracking: true
            );

            if (location == null) return null;

            var viewModel = Mapper.Map<LocationViewModel>(location);
            var translation = location.Translations?.FirstOrDefault();

            if (translation != null)
            {
                viewModel.Name = translation.Name;
            }

            return viewModel;
        }

        public override async Task<IEnumerable<LocationViewModel>> GetAllAsync(
            Expression<Func<Location, bool>>? predicate = null,
            Func<IQueryable<Location>, IOrderedQueryable<Location>>? orderBy = null,
            Func<IQueryable<Location>, IIncludableQueryable<Location, object>>? include = null,
            bool AsNoTracking = false)
        {
            var languageId = await _cookieService.GetLanguageIdAsync();

            Func<IQueryable<Location>, IIncludableQueryable<Location, object>> includeWithTranslations = query =>
            {
                var included = query
                    .Include(l => l.Translations.Where(t => t.LanguageId == languageId))
                    .Include(l => l.Sports)
                    .Include(l => l.Events);

                return include != null ? include(included) : included;
            };

            var locations = await Repository.GetAllAsync(predicate, orderBy, includeWithTranslations, AsNoTracking);

            return locations.Select(l =>
            {
                var viewModel = Mapper.Map<LocationViewModel>(l);
                var translation = l.Translations?.FirstOrDefault();

                if (translation != null)
                {
                    viewModel.Name = translation.Name;
                }

                viewModel.SportCount = l.Sports?.Count(s => s.IsActive && s.EventDate > DateTime.Now) ?? 0;
                viewModel.EventCount = l.Events?.Count(e => e.IsActive && e.EventDate > DateTime.Now) ?? 0;

                return viewModel;
            });
        }

        public async Task<IEnumerable<LocationOption>> GetLocationsForFilterAsync(int languageId)
        {
            var locations = await Repository.GetAllAsync(
                include: query => query
                    .Include(l => l.Translations.Where(t => t.LanguageId == languageId))
                    .Include(l => l.Sports)
                    .Include(l => l.Events),
                AsNoTracking: true
            );

            return locations.Select(l => new LocationOption
            {
                Id = l.Id,
                Name = l.Translations.FirstOrDefault()?.Name ?? "Unknown",
                Count = (l.Sports?.Count(s => s.IsActive && s.EventDate > DateTime.Now) ?? 0) +
                       (l.Events?.Count(e => e.IsActive && e.EventDate > DateTime.Now) ?? 0)
            })
            .Where(l => l.Count > 0)
            .OrderByDescending(l => l.Count);
        }

        public async Task<string?> GetLocationNameAsync(int locationId, int languageId)
        {
            var location = await Repository.GetAsync(
                predicate: l => l.Id == locationId,
                include: query => query.Include(l => l.Translations.Where(t => t.LanguageId == languageId)),
                AsNoTracking: true
            );

            return location?.Translations?.FirstOrDefault()?.Name;
        }
    }
}