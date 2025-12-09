using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.EventCategory;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Implementations
{
    public class EventCategoryManager : CrudManager<EventCategory, EventCategoryViewModel, EventCategoryCreateViewModel, EventCategoryUpdateViewModel>, IEventCategoryService
    {
        private readonly ICookieService _cookieService;

        public EventCategoryManager(
            IRepositoryAsync<EventCategory> repository,
            IMapper mapper,
            ICookieService cookieService)
            : base(repository, mapper)
        {
            _cookieService = cookieService;
        }

        public async Task<IEnumerable<EventCategoryOption>> GetCategoriesForFilterAsync(int languageId)
        {
            var categories = await Repository.GetAllAsync(
                include: query => query
                    .Include(c => c.Translations.Where(t => t.LanguageId == languageId))
                    .Include(c => c.Events),
                AsNoTracking: true
            );

            return categories.Select(c => new EventCategoryOption
            {
                Id = c.Id,
                Name = c.Translations.FirstOrDefault()?.Name ?? "Unknown",
                Count = c.Events?.Count(e => e.IsActive) ?? 0
            }).Where(c => c.Count > 0)
              .OrderByDescending(c => c.Count);
        }

        public async Task<string?> GetCategoryNameAsync(int categoryId, int languageId)
        {
            var category = await Repository.GetAsync(
                predicate: c => c.Id == categoryId,
                include: query => query.Include(c => c.Translations.Where(t => t.LanguageId == languageId)),
                AsNoTracking: true
            );

            return category?.Translations?.FirstOrDefault()?.Name;
        }

        public override async Task<EventCategoryViewModel> CreateAsync(EventCategoryCreateViewModel model)
        {
            var category = new EventCategory
            {
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            var currentLanguageId = await _cookieService.GetLanguageIdAsync();

            category.Translations = new List<EventCategoryTranslation>
            {
                new EventCategoryTranslation
                {
                    Name = model.Name,
                    LanguageId = currentLanguageId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };

            var createdCategory = await Repository.AddAsync(category);
            return Mapper.Map<EventCategoryViewModel>(createdCategory);
        }

        public override async Task<bool> UpdateAsync(int id, EventCategoryUpdateViewModel model)
        {
            var category = await Repository.GetAsync(
                predicate: c => c.Id == id,
                include: query => query.Include(c => c.Translations)
            );

            if (category == null) return false;

            var currentLanguageId = await _cookieService.GetLanguageIdAsync();
            var translation = category.Translations.FirstOrDefault(t => t.LanguageId == currentLanguageId);

            if (translation != null)
            {
                translation.Name = model.Name;
                translation.UpdatedAt = DateTime.Now;
            }
            else
            {
                category.Translations.Add(new EventCategoryTranslation
                {
                    Name = model.Name,
                    LanguageId = currentLanguageId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
            }

            category.UpdatedAt = DateTime.Now;
            await Repository.UpdateAsync(category);
            return true;
        }
    }

}
