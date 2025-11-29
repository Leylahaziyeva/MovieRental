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
        private readonly ICookieService _cookieService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IRepositoryAsync<Person> _personRepository;

        public EventManager(
            IRepositoryAsync<Event> repository,
            IMapper mapper,
            ICookieService cookieService,
            ICloudinaryService cloudinaryService,
            IRepositoryAsync<Person> personRepository)
            : base(repository, mapper)
        {
            _cookieService = cookieService;
            _cloudinaryService = cloudinaryService;
            _personRepository = personRepository;
        }

        public async Task<IEnumerable<EventViewModel>> GetUpcomingEventsAsync(int languageId)
        {
            var now = DateTime.Now;

            var events = await Repository.GetAllAsync(
                predicate: e => e.IsActive && e.EventDate >= now,
                include: query => query
                    .Include(e => e.EventTranslations.Where(et => et.LanguageId == languageId))
                    .Include(e => e.Currency)
                    .Include(e => e.Artists)!
                        .ThenInclude(a => a.PersonTranslations!.Where(pt => pt.LanguageId == languageId)),
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
                    .Include(e => e.Currency)
                    .Include(e => e.Artists)!
                        .ThenInclude(a => a.PersonTranslations!.Where(pt => pt.LanguageId == languageId)),
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
                    .Include(e => e.Currency)
                    .Include(e => e.Artists)!
                        .ThenInclude(a => a.PersonTranslations!.Where(pt => pt.LanguageId == filter.CurrentLanguageId)),
                AsNoTracking: true
            );

            if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
            {
                query = query.Where(e =>
                    e.EventTranslations.Any(et =>
                        et.Name.Contains(filter.SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                        et.Description.Contains(filter.SearchQuery, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(filter.Location))
            {
                query = query.Where(e =>
                    e.EventTranslations.Any(et =>
                        et.Location.Contains(filter.Location, StringComparison.OrdinalIgnoreCase))
                ).ToList();
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
                query = query.Where(e => e.Price.HasValue && e.Price <= filter.MaxPrice.Value).ToList();
            }

            if (!string.IsNullOrWhiteSpace(filter.Category))
            {
                query = query.Where(e =>
                    e.EventTranslations.Any(et =>
                        et.Categories != null &&
                        et.Categories.Contains(filter.Category, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            var totalCount = query.Count();

            var paginatedEvents = query
                .OrderBy(e => e.EventDate)
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToList();

            var events = Mapper.Map<List<EventViewModel>>(paginatedEvents);

            return new EventFilterResultViewModel
            {
                Events = events,
                TotalCount = totalCount,
                Filter = filter
            };
        }

        public override async Task<EventViewModel> CreateAsync(EventCreateViewModel model)
        {
            if (model.ImageFile != null)
            {
                model.ImageUrl = await _cloudinaryService.ImageCreateAsync(model.ImageFile);
            }

            if (model.CoverImageFile != null)
            {
                model.CoverImageUrl = await _cloudinaryService.ImageCreateAsync(model.CoverImageFile);
            }

            var eventEntity = Mapper.Map<Event>(model);
            eventEntity.CreatedAt = DateTime.Now;
            eventEntity.UpdatedAt = DateTime.Now;

            var currentLanguageId = await _cookieService.GetLanguageIdAsync();

            eventEntity.EventTranslations = new List<EventTranslation>
            {
                new EventTranslation
                {
                    Name = model.Name,
                    Description = model.Description,
                    Location = model.Location,
                    Categories = model.Categories,
                    Languages = model.Languages,
                    LanguageId = currentLanguageId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };

            if (model.SelectedArtistIds != null && model.SelectedArtistIds.Any())
            {
                eventEntity.Artists = new List<Person>();
                foreach (var artistId in model.SelectedArtistIds)
                {
                    var artist = await _personRepository.GetByIdAsync(artistId);
                    if (artist != null)
                    {
                        eventEntity.Artists.Add(artist);
                    }
                }
            }

            var createdEntity = await Repository.AddAsync(eventEntity);
            return Mapper.Map<EventViewModel>(createdEntity);
        }

        public override async Task<bool> UpdateAsync(int id, EventUpdateViewModel model)
        {
            var eventEntity = await Repository.GetAsync(
                predicate: e => e.Id == id,
                include: query => query
                    .Include(e => e.EventTranslations)
                    .Include(e => e.Artists!)
            );

            if (eventEntity == null)
                return false;

            if (model.ImageFile != null)
            {
                if (!string.IsNullOrEmpty(eventEntity.ImageUrl))
                {
                    await _cloudinaryService.ImageDeleteAsync(eventEntity.ImageUrl);
                }
                model.ImageUrl = await _cloudinaryService.ImageCreateAsync(model.ImageFile);
            }

            if (model.CoverImageFile != null)
            {
                if (!string.IsNullOrEmpty(eventEntity.CoverImageUrl))
                {
                    await _cloudinaryService.ImageDeleteAsync(eventEntity.CoverImageUrl);
                }
                model.CoverImageUrl = await _cloudinaryService.ImageCreateAsync(model.CoverImageFile);
            }

            Mapper.Map(model, eventEntity);
            eventEntity.UpdatedAt = DateTime.Now;

            var currentLanguageId = await _cookieService.GetLanguageIdAsync();
            var existingTranslation = eventEntity.EventTranslations
                .FirstOrDefault(et => et.LanguageId == currentLanguageId);

            if (existingTranslation != null)
            {
                existingTranslation.Name = model.Name!;
                existingTranslation.Description = model.Description!;
                existingTranslation.Location = model.Location!;
                existingTranslation.Categories = model.Categories;
                existingTranslation.Languages = model.Languages;
                existingTranslation.UpdatedAt = DateTime.Now;
            }
            else
            {
                eventEntity.EventTranslations.Add(new EventTranslation
                {
                    Name = model.Name!,
                    Description = model.Description!,
                    Location = model.Location!,
                    Categories = model.Categories,
                    Languages = model.Languages,
                    LanguageId = currentLanguageId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
            }

            if (model.SelectedArtistIds != null)
            {
                eventEntity.Artists?.Clear();
                eventEntity.Artists = new List<Person>();

                foreach (var artistId in model.SelectedArtistIds)
                {
                    var artist = await _personRepository.GetByIdAsync(artistId);
                    if (artist != null)
                    {
                        eventEntity.Artists.Add(artist);
                    }
                }
            }

            await Repository.UpdateAsync(eventEntity);
            return true;
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var eventEntity = await Repository.GetByIdAsync(id);
            if (eventEntity == null) return false;

            if (!string.IsNullOrEmpty(eventEntity.ImageUrl))
            {
                await _cloudinaryService.ImageDeleteAsync(eventEntity.ImageUrl);
            }

            if (!string.IsNullOrEmpty(eventEntity.CoverImageUrl))
            {
                await _cloudinaryService.ImageDeleteAsync(eventEntity.CoverImageUrl);
            }

            return await base.DeleteAsync(id);
        }

        public async Task<bool> AddArtistsToEventAsync(int eventId, List<int> artistIds)
        {
            var eventEntity = await Repository.GetAsync(
                predicate: x => x.Id == eventId,
                include: query => query.Include(e => e.Artists!)
            );

            if (eventEntity == null) return false;

            foreach (var artistId in artistIds)
            {
                var artist = await _personRepository.GetByIdAsync(artistId);
                if (artist != null && !eventEntity.Artists!.Any(a => a.Id == artistId))
                {
                    eventEntity.Artists!.Add(artist);
                }
            }

            await Repository.UpdateAsync(eventEntity);
            return true;
        }

        public async Task<bool> RemoveArtistFromEventAsync(int eventId, int artistId)
        {
            var eventEntity = await Repository.GetAsync(
                predicate: x => x.Id == eventId,
                include: query => query.Include(e => e.Artists!)
            );

            if (eventEntity == null) return false;

            var artist = eventEntity.Artists?.FirstOrDefault(a => a.Id == artistId);
            if (artist != null)
            {
                eventEntity.Artists!.Remove(artist);
                await Repository.UpdateAsync(eventEntity);
                return true;
            }

            return false;
        }
    }
}