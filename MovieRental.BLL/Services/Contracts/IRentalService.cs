using MovieRental.BLL.ViewModels.Rental;

namespace MovieRental.BLL.Services.Contracts
{
    public interface IRentalService
    {
        Task<RentalResultViewModel> RentMovieAsync(string userId, int movieId);
        Task<bool> CancelRentalAsync(int rentalId, string userId);

        Task<List<RentalViewModel>> GetUserActiveRentalsAsync(string userId);
        Task<List<RentalViewModel>> GetUserExpiredRentalsAsync(string userId);
        Task<RentalViewModel?> GetRentalByIdAsync(int rentalId, string userId);

        Task<bool> HasActiveRentalAsync(string userId, int movieId);
        Task<RentalViewModel?> GetActiveRentalForMovieAsync(string userId, int movieId);

        Task<bool> RecordWatchAsync(string userId, int movieId, int watchDurationSeconds);
        Task<List<WatchHistoryViewModel>> GetWatchHistoryAsync(string userId, int page = 1, int pageSize = 20);

        Task<int> ExpireOldRentalsAsync();
    }
}