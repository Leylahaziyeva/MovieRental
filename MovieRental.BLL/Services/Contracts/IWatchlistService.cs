using MovieRental.BLL.ViewModels.Watchlist;

namespace MovieRental.BLL.Services.Contracts
{
    public interface IWatchlistService
    {
        Task<WatchlistViewModel> GetUserWatchlistAsync(string userId, int page = 1, int pageSize = 12);
        Task<bool> AddToWatchlistAsync(string userId, int movieId);
        Task<bool> RemoveFromWatchlistAsync(string userId, int movieId);
        Task<bool> IsInWatchlistAsync(string userId, int movieId);
    }
}