using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Event;
using MovieRental.BLL.ViewModels.Person;
using MovieRental.DAL.DataContext.Entities;
using System.Linq.Expressions;

namespace MovieRental.BLL.Services.Implementations
{
    public class EventManager : CrudManager<Event, EventViewModel, EventCreateViewModel, EventUpdateViewModel>, IEventService
    {
        private readonly ICookieService _cookieService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IRepositoryAsync<Person> _personRepository;
        private readonly ILocationService _locationService;
        private readonly IEventCategoryService _eventCategoryService;

        public EventManager(
            IRepositoryAsync<Event> repository,
            IMapper mapper,
            ICookieService cookieService,
            ICloudinaryService cloudinaryService,
            IRepositoryAsync<Person> personRepository,
            ILocationService locationService,
            IEventCategoryService eventCategoryService)
            : base(repository, mapper)
        {
            _cookieService = cookieService;
            _cloudinaryService = cloudinaryService;
            _personRepository = personRepository;
            _locationService = locationService;
            _eventCategoryService = eventCategoryService;
        }

        #region Get Methods - FIXED

        public async Task<IEnumerable<EventViewModel>> GetUpcomingEventsAsync(int languageId)
        {
            var now = DateTime.Now;

            var events = await Repository.GetAllAsync(
                predicate: e => e.IsActive && e.EventDate >= now,
                include: query => query
                    .Include(e => e.EventTranslations.Where(et => et.LanguageId == languageId))
                    .Include(e => e.Currency)
                    .Include(e => e.EventCategory!)
                        .ThenInclude(ec => ec.Translations.Where(t => t.LanguageId == languageId))
                    .Include(e => e.Location!)
                        .ThenInclude(l => l.Translations.Where(t => t.LanguageId == languageId))
                    .Include(e => e.Artists!) // FIXED: Remove filter here
                        .ThenInclude(a => a.PersonTranslations!.Where(pt => pt.LanguageId == languageId)),
                AsNoTracking: true
            );

            return await MapToViewModelsAsync(events.ToList(), languageId);
        }

        public async Task<EventViewModel?> GetEventByIdWithTranslationsAsync(int id, int languageId)
        {
            var eventEntity = await Repository.GetAsync(
                predicate: e => e.Id == id,
                include: query => query
                    .Include(e => e.EventTranslations.Where(et => et.LanguageId == languageId))
                    .Include(e => e.Currency)
                    .Include(e => e.EventCategory!)
                        .ThenInclude(ec => ec.Translations.Where(t => t.LanguageId == languageId))
                    .Include(e => e.Location!)
                        .ThenInclude(l => l.Translations.Where(t => t.LanguageId == languageId))
                    .Include(e => e.Artists!) // FIXED: Load all artists
                        .ThenInclude(a => a.PersonTranslations!.Where(pt => pt.LanguageId == languageId)),
                AsNoTracking: true
            );

            if (eventEntity == null) return null;

            // Debug logging
            Console.WriteLine($"Event loaded: {eventEntity.Id}");
            Console.WriteLine($"Artists count: {eventEntity.Artists?.Count ?? 0}");
            if (eventEntity.Artists != null)
            {
                foreach (var artist in eventEntity.Artists)
                {
                    Console.WriteLine($"- Artist: {artist.Id}, Type: {artist.PersonType}");
                }
            }

            var viewModels = await MapToViewModelsAsync(new List<Event> { eventEntity }, languageId);
            return viewModels.FirstOrDefault();
        }

        #endregion

        #region Filtering & Pagination - FIXED

        public async Task<EventFilterResultViewModel> GetFilteredEventsAsync(EventFilterViewModel filter)
        {
            Expression<Func<Event, bool>> predicate = e => e.IsActive;

            if (!string.IsNullOrWhiteSpace(filter.SearchQuery))
            {
                predicate = predicate.And(e =>
                    e.EventTranslations.Any(et =>
                        et.LanguageId == filter.CurrentLanguageId &&
                        (et.Name.Contains(filter.SearchQuery) ||
                         et.Description.Contains(filter.SearchQuery))));
            }

            if (filter.LocationId.HasValue)
            {
                predicate = predicate.And(e => e.LocationId == filter.LocationId.Value);
            }

            if (filter.EventCategoryId.HasValue)
            {
                predicate = predicate.And(e => e.EventCategoryId == filter.EventCategoryId.Value);
            }

            if (filter.FromDate.HasValue)
            {
                predicate = predicate.And(e => e.EventDate >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                predicate = predicate.And(e => e.EventDate <= filter.ToDate.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                predicate = predicate.And(e => e.Price.HasValue && e.Price <= filter.MaxPrice.Value);
            }

            Func<IQueryable<Event>, IIncludableQueryable<Event, object>> include = query =>
                query.Include(e => e.EventTranslations.Where(et => et.LanguageId == filter.CurrentLanguageId))
                     .Include(e => e.Currency)
                     .Include(e => e.EventCategory!)
                         .ThenInclude(ec => ec.Translations.Where(t => t.LanguageId == filter.CurrentLanguageId))
                     .Include(e => e.Location!)
                         .ThenInclude(l => l.Translations.Where(t => t.LanguageId == filter.CurrentLanguageId))
                     .Include(e => e.Artists!) // FIXED: Remove PersonType filter
                         .ThenInclude(a => a.PersonTranslations!.Where(pt => pt.LanguageId == filter.CurrentLanguageId));

            var (items, totalCount) = await Repository.GetPagedAsync(
                predicate: predicate,
                orderBy: query => query.OrderBy(e => e.EventDate),
                include: include,
                page: filter.Page,
                pageSize: filter.PageSize,
                AsNoTracking: true
            );

            var events = await MapToViewModelsAsync(items.ToList(), filter.CurrentLanguageId);

            return new EventFilterResultViewModel
            {
                Events = events.ToList(),
                TotalCount = totalCount,
                Filter = filter
            };
        }

        public async Task<(IEnumerable<EventViewModel> Events, int TotalCount)> GetEventsPagedAsync(EventFilterViewModel filter)
        {
            var result = await GetFilteredEventsAsync(filter);
            return (result.Events, result.TotalCount);
        }

        public async Task<EventFilterViewModel> GetFilterOptionsAsync(int languageId)
        {
            var categories = await _eventCategoryService.GetCategoriesForFilterAsync(languageId);
            var locations = await _locationService.GetLocationsForFilterAsync(languageId);

            return new EventFilterViewModel
            {
                CurrentLanguageId = languageId,
                Categories = categories.ToList(),
                Locations = locations.ToList()
            };
        }

        #endregion

        #region Create & Update - FIXED

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
                    LanguageId = currentLanguageId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };

            // FIXED: Load artists properly
            if (model.SelectedArtistIds != null && model.SelectedArtistIds.Any())
            {
                var artists = await _personRepository.GetAllAsync(
                    predicate: p => model.SelectedArtistIds.Contains(p.Id)
                );
                eventEntity.Artists = artists.ToList();

                Console.WriteLine($"Creating event with {artists.Count()} artists");
            }

            var createdEntity = await Repository.AddAsync(eventEntity);

            var viewModel = await GetEventByIdWithTranslationsAsync(createdEntity.Id, currentLanguageId);
            return viewModel!;
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
                existingTranslation.UpdatedAt = DateTime.Now;
            }
            else
            {
                eventEntity.EventTranslations.Add(new EventTranslation
                {
                    Name = model.Name!,
                    Description = model.Description!,
                    LanguageId = currentLanguageId,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                });
            }

            // FIXED: Update artists properly
            if (model.SelectedArtistIds != null)
            {
                eventEntity.Artists?.Clear();

                if (model.SelectedArtistIds.Any())
                {
                    var artists = await _personRepository.GetAllAsync(
                        predicate: p => model.SelectedArtistIds.Contains(p.Id)
                    );
                    eventEntity.Artists = artists.ToList();

                    Console.WriteLine($"Updating event with {artists.Count()} artists");
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

        #endregion

        #region Artist Management - FIXED

        public async Task<bool> AddArtistsToEventAsync(int eventId, List<int> artistIds)
        {
            var eventEntity = await Repository.GetAsync(
                predicate: x => x.Id == eventId,
                include: query => query.Include(e => e.Artists!)
            );

            if (eventEntity == null) return false;

            // FIXED: Load all requested artists at once
            var artists = await _personRepository.GetAllAsync(
                predicate: p => artistIds.Contains(p.Id)
            );

            foreach (var artist in artists)
            {
                if (!eventEntity.Artists!.Any(a => a.Id == artist.Id))
                {
                    eventEntity.Artists!.Add(artist);
                    Console.WriteLine($"Added artist {artist.Id} to event {eventId}");
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
                Console.WriteLine($"Removed artist {artistId} from event {eventId}");
                return true;
            }

            return false;
        }

        #endregion

        #region Helper Methods - FIXED

        private async Task<IEnumerable<EventViewModel>> MapToViewModelsAsync(List<Event> events, int languageId)
        {
            var viewModels = new List<EventViewModel>();

            foreach (var eventEntity in events)
            {
                var viewModel = Mapper.Map<EventViewModel>(eventEntity);

                var translation = eventEntity.EventTranslations?.FirstOrDefault();
                if (translation != null)
                {
                    viewModel.Name = translation.Name;
                    viewModel.Description = translation.Description;
                }

                if (eventEntity.EventCategory != null)
                {
                    var categoryTranslation = eventEntity.EventCategory.Translations?.FirstOrDefault();
                    viewModel.CategoryName = categoryTranslation?.Name;
                }

                if (eventEntity.Location != null)
                {
                    var locationTranslation = eventEntity.Location.Translations?.FirstOrDefault();
                    viewModel.LocationName = locationTranslation?.Name;
                }

                // FIXED: Map all artists without filtering
                if (eventEntity.Artists?.Any() == true)
                {
                    viewModel.Artists = new List<PersonViewModel>();
                    foreach (var artist in eventEntity.Artists)
                    {
                        var personViewModel = Mapper.Map<PersonViewModel>(artist);
                        var personTranslation = artist.PersonTranslations?.FirstOrDefault();
                        if (personTranslation != null)
                        {
                            personViewModel.Name = personTranslation.Name;
                            personViewModel.Biography = personTranslation.Biography;
                        }
                        viewModel.Artists.Add(personViewModel);

                        Console.WriteLine($"Mapped artist: {personViewModel.Name} (ID: {artist.Id})");
                    }
                }

                if (eventEntity.Price.HasValue && eventEntity.Currency != null)
                {
                    viewModel.FormattedPrice = $"{eventEntity.Currency.Symbol}{eventEntity.Price.Value:N2}";
                }

                viewModels.Add(viewModel);
            }

            return viewModels;
        }

        #endregion
    }
}