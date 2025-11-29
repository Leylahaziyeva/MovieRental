using MovieRental.BLL.ViewModels.Sport;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Contracts
{
    public interface ISportService : ICrudService<Sport, SportViewModel, SportCreateViewModel, SportUpdateViewModel>
    {
        Task<IEnumerable<SportViewModel>> GetFeaturedSportsAsync(int count = 4);
        Task<IEnumerable<SportViewModel>> GetUpcomingSportsAsync();
        Task<IEnumerable<SportViewModel>> GetPastSportsAsync();
        Task<IEnumerable<SportViewModel>> GetSportsByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<SportViewModel>> GetSportsByLocationAsync(string location);
        Task<IEnumerable<SportViewModel>> GetSportsByCategoryAsync(string category);
        Task<SportViewModel?> GetSportDetailAsync(int id);
        Task<bool> AddPlayersToSportAsync(int sportId, List<int> playerIds);
        Task<bool> RemovePlayerFromSportAsync(int sportId, int playerId);
    }
}