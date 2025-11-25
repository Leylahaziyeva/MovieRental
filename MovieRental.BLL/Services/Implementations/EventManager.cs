using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Event;
using MovieRental.DAL.DataContext.Entities;
using MovieRental.DAL.Repositories.Contracts;

namespace MovieRental.BLL.Services.Implementations
{
    public class EventManager : CrudManager<Event, EventViewModel, EventCreateViewModel, EventUpdateViewModel>, IEventService
    {
        public EventManager(IRepositoryAsync<Event> repository, IMapper mapper)
            : base(repository, mapper)
        {
        }

        public async Task<IEnumerable<EventViewModel>> GetUpcomingEventsAsync(int languageId)
        {
            var now = DateTime.Now;

            var events = await Repository.GetAllAsync(
                predicate: e => e.IsActive && e.EventDate >= now,
                include: query => query
                    .Include(e => e.EventTranslations.Where(et => et.LanguageId == languageId))
                    .Include(e => e.Currency)!,
                orderBy: query => query.OrderBy(e => e.EventDate),
                AsNoTracking: true
            );

            return Mapper.Map<IEnumerable<EventViewModel>>(events);
        }

        public async Task<EventViewModel?> GetEventByIdWithTranslationsAsync(int id, int languageId)
        {
            var eventEntity = await Repository.GetAsync(
                predicate: e => e.Id == id,
                include: query => query
                    .Include(e => e.EventTranslations.Where(et => et.LanguageId == languageId))
                    .Include(e => e.Currency)!,
                AsNoTracking: true
            );

            return Mapper.Map<EventViewModel>(eventEntity);
        }

        public async Task<EventFilterResultViewModel> GetFilteredEventsAsync(EventFilterViewModel filter)
        {
            var query = await Repository.GetAllAsync(
                predicate: e => e.IsActive,
                include: q => q
                    .Include(e => e.EventTranslations.Where(et => et.LanguageId == filter.CurrentLanguageId))
                    .Include(e => e.Currency)!,
                AsNoTracking: true
            );

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
            {
                query = query.Where(e =>
                    e.EventTranslations.Any(et => et.Name.Contains(filter.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                                                   et.Description.Contains(filter.SearchQuery, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(filter.Location))
            {
                query = query.Where(e => e.Location.Contains(filter.Location, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (filter.FromDate.HasValue)
            {
                query = query.Where(e => e.EventDate >= filter.FromDate.Value).ToList();
            }

            if (filter.ToDate.HasValue)
            {
                query = query.Where(e => e.EventDate <= filter.ToDate.Value).ToList();
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(e => e.Price <= filter.MaxPrice.Value).ToList();
            }

            var events = Mapper.Map<List<EventViewModel>>(query);

            return new EventFilterResultViewModel
            {
                Events = events,
                TotalCount = events.Count,
                Filter = filter
            };
        }

        public override async Task<EventViewModel> CreateAsync(EventCreateViewModel model)
        {
            var eventEntity = Mapper.Map<Event>(model);
            eventEntity.CreatedAt = DateTime.Now;
            eventEntity.UpdatedAt = DateTime.Now;

            if (model.Translations != null && model.Translations.Any())
            {
                eventEntity.EventTranslations = model.Translations.Select(t => new EventTranslation
                {
                    Name = t.Name,
                    Description = t.Description,
                    LanguageId = t.LanguageId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }).ToList();
            }

            var createdEntity = await Repository.AddAsync(eventEntity);
            return Mapper.Map<EventViewModel>(createdEntity);
        }

        public override async Task<bool> UpdateAsync(int id, EventUpdateViewModel model)
        {
            var eventEntity = await Repository.GetAsync(
                predicate: e => e.Id == id,
                include: query => query.Include(e => e.EventTranslations)
            );

            if (eventEntity == null)
                return false;

            Mapper.Map(model, eventEntity);
            eventEntity.UpdatedAt = DateTime.Now;

            if (model.Translations != null)
            {
                eventEntity.EventTranslations.Clear();

                foreach (var translation in model.Translations)
                {
                    eventEntity.EventTranslations.Add(new EventTranslation
                    {
                        Name = translation.Name,
                        Description = translation.Description,
                        LanguageId = translation.LanguageId,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    });
                }
            }

            await Repository.UpdateAsync(eventEntity);
            return true;
        }
    }
}