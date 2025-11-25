using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Sport;
using MovieRental.DAL.DataContext.Entities;
using MovieRental.DAL.Repositories.Contracts;

namespace MovieRental.BLL.Services.Implementations
{
    public class SportManager : CrudManager<Sport, SportViewModel, SportCreateViewModel, SportUpdateViewModel>, ISportService
    {
        public SportManager(IRepositoryAsync<Sport> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }

        public async Task<IEnumerable<SportViewModel>> GetUpcomingSportsAsync(int languageId)
        {
            var now = DateTime.Now;

            var sports = await Repository.GetAllAsync(
                predicate: s => s.IsActive && s.EventDate >= now,
                include: query => query
                    .Include(s => s.SportTranslations.Where(st => st.LanguageId == languageId))
                    .Include(s => s.Currency)!,
                orderBy: query => query.OrderBy(s => s.EventDate),
                AsNoTracking: true
            );

            return Mapper.Map<IEnumerable<SportViewModel>>(sports);
        }

        public async Task<SportViewModel?> GetSportByIdWithTranslationsAsync(int id, int languageId)
        {
            var sport = await Repository.GetAsync(
                predicate: s => s.Id == id,
                include: query => query
                    .Include(s => s.SportTranslations.Where(st => st.LanguageId == languageId))
                    .Include(s => s.Currency)!,
                AsNoTracking: true
            );

            return Mapper.Map<SportViewModel>(sport);
        }

        public async Task<SportFilterResultViewModel> GetFilteredSportsAsync(SportFilterViewModel filter)
        {
            var query = await Repository.GetAllAsync(
                predicate: s => s.IsActive,
                include: q => q
                    .Include(s => s.SportTranslations.Where(st => st.LanguageId == filter.CurrentLanguageId))
                    .Include(s => s.Currency)!,
                AsNoTracking: true
            );

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
            {
                query = query.Where(s =>
                    s.SportTranslations.Any(st => st.Name.Contains(filter.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                                                   st.Description.Contains(filter.SearchQuery, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(filter.Location))
            {
                query = query.Where(s => s.Location.Contains(filter.Location, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (filter.FromDate.HasValue)
            {
                query = query.Where(s => s.EventDate >= filter.FromDate.Value).ToList();
            }

            if (filter.ToDate.HasValue)
            {
                query = query.Where(s => s.EventDate <= filter.ToDate.Value).ToList();
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(s => s.Price <= filter.MaxPrice.Value).ToList();
            }

            var sports = Mapper.Map<List<SportViewModel>>(query);

            return new SportFilterResultViewModel
            {
                Sports = sports,
                TotalCount = sports.Count,
                Filter = filter
            };
        }

        public override async Task<SportViewModel> CreateAsync(SportCreateViewModel model)
        {
            var sport = Mapper.Map<Sport>(model);
            sport.CreatedAt = DateTime.Now;
            sport.UpdatedAt = DateTime.Now;

            if (model.Translations != null && model.Translations.Any())
            {
                sport.SportTranslations = model.Translations.Select(t => new SportTranslation
                {
                    Name = t.Name,
                    Description = t.Description,
                    LanguageId = t.LanguageId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }).ToList();
            }

            var createdSport = await Repository.AddAsync(sport);
            return Mapper.Map<SportViewModel>(createdSport);
        }

        public override async Task<bool> UpdateAsync(int id, SportUpdateViewModel model)
        {
            var sport = await Repository.GetAsync(
                predicate: s => s.Id == id,
                include: query => query.Include(s => s.SportTranslations)
            );

            if (sport == null)
                return false;

            Mapper.Map(model, sport);
            sport.UpdatedAt = DateTime.Now;

            if (model.Translations != null)
            {
                sport.SportTranslations.Clear();

                foreach (var translation in model.Translations)
                {
                    sport.SportTranslations.Add(new SportTranslation
                    {
                        Name = translation.Name,
                        Description = translation.Description,
                        LanguageId = translation.LanguageId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    });
                }
            }

            await Repository.UpdateAsync(sport);
            return true;
        }
    }
}