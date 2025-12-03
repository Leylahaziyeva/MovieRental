using MovieRental.BLL.ViewModels.Event;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Contracts
{
    public interface IEventService : ICrudService<Event, EventViewModel, EventCreateViewModel, EventUpdateViewModel>
    {
        Task<IEnumerable<EventViewModel>> GetUpcomingEventsAsync(int languageId);
        Task<EventViewModel?> GetEventByIdWithTranslationsAsync(int id, int languageId);
        Task<EventFilterResultViewModel> GetFilteredEventsAsync(EventFilterViewModel filter);
        Task<bool> AddArtistsToEventAsync(int eventId, List<int> artistIds);
        Task<bool> RemoveArtistFromEventAsync(int eventId, int artistId);
        Task<(IEnumerable<EventViewModel> Events, int TotalCount)> GetEventsPagedAsync(EventFilterViewModel filter);
    }
}