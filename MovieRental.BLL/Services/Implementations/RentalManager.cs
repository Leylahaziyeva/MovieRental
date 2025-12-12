using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Rental;
using MovieRental.DAL.DataContext;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Implementations
{
    public class RentalManager : IRentalService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RentalManager> _logger;

        public RentalManager(AppDbContext context, ILogger<RentalManager> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<RentalResultViewModel> RentMovieAsync(string userId, int movieId)
        {
            try
            {
                var movie = await _context.Movies
                    .Include(m => m.Currency)
                    .FirstOrDefaultAsync(m => m.Id == movieId && m.IsAvailableForRent);

                if (movie == null)
                {
                    return new RentalResultViewModel
                    {
                        Success = false,
                        Message = "Movie not available for rent"
                    };
                }

                var existingRental = await _context.Rentals
                    .FirstOrDefaultAsync(r =>
                        r.UserId == userId &&
                        r.MovieId == movieId &&
                        r.Status == RentalStatus.Active &&
                        r.ExpiryDate > DateTime.UtcNow);

                if (existingRental != null)
                {
                    return new RentalResultViewModel
                    {
                        Success = false,
                        Message = "You already have an active rental for this movie"
                    };
                }

                var rental = new Rental
                {
                    UserId = userId,
                    MovieId = movieId,
                    RentalDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddDays(movie.RentalDurationDays),
                    Price = movie.RentalPrice,
                    Status = RentalStatus.Active,
                    IsPaid = true, // Sonra Stripe ilə dəyişəcəyik
                    PaidAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Rentals.Add(rental);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"User {userId} rented movie {movieId}");

                return new RentalResultViewModel
                {
                    Success = true,
                    Message = "Movie rented successfully",
                    RentalId = rental.Id,
                    ExpiryDate = rental.ExpiryDate,
                    Price = rental.Price
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error renting movie {movieId} for user {userId}");
                return new RentalResultViewModel
                {
                    Success = false,
                    Message = "An error occurred while renting the movie"
                };
            }
        }

        public async Task<List<RentalViewModel>> GetUserActiveRentalsAsync(string userId)
        {
            var rentals = await _context.Rentals
                .Include(r => r.Movie)
                    .ThenInclude(m => m!.MovieTranslations)
                .Include(r => r.Movie!.Currency)
                .Where(r => r.UserId == userId &&
                           r.Status == RentalStatus.Active &&
                           r.ExpiryDate > DateTime.UtcNow)
                .OrderByDescending(r => r.RentalDate)
                .ToListAsync();

            return rentals.Select(r => MapToViewModel(r)).ToList();
        }

        public async Task<List<RentalViewModel>> GetUserExpiredRentalsAsync(string userId)
        {
            var rentals = await _context.Rentals
                .Include(r => r.Movie)
                    .ThenInclude(m => m!.MovieTranslations)
                .Include(r => r.Movie!.Currency)
                .Where(r => r.UserId == userId &&
                           (r.Status == RentalStatus.Expired || r.ExpiryDate <= DateTime.UtcNow))
                .OrderByDescending(r => r.RentalDate)
                .Take(50)
                .ToListAsync();

            return rentals.Select(r => MapToViewModel(r)).ToList();
        }

        public async Task<bool> HasActiveRentalAsync(string userId, int movieId)
        {
            return await _context.Rentals
                .AnyAsync(r =>
                    r.UserId == userId &&
                    r.MovieId == movieId &&
                    r.Status == RentalStatus.Active &&
                    r.ExpiryDate > DateTime.UtcNow);
        }

        public async Task<RentalViewModel?> GetActiveRentalForMovieAsync(string userId, int movieId)
        {
            var rental = await _context.Rentals
                .Include(r => r.Movie)
                    .ThenInclude(m => m!.MovieTranslations)
                .Include(r => r.Movie!.Currency)
                .FirstOrDefaultAsync(r =>
                    r.UserId == userId &&
                    r.MovieId == movieId &&
                    r.Status == RentalStatus.Active &&
                    r.ExpiryDate > DateTime.UtcNow);

            return rental != null ? MapToViewModel(rental) : null;
        }

        public async Task<bool> CancelRentalAsync(int rentalId, string userId)
        {
            var rental = await _context.Rentals
                .FirstOrDefaultAsync(r => r.Id == rentalId && r.UserId == userId);

            if (rental == null) return false;

            rental.Status = RentalStatus.Cancelled;
            rental.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RecordWatchAsync(string userId, int movieId, int watchDurationSeconds)
        {
            try
            {
                var movie = await _context.Movies.FindAsync(movieId);
                if (movie == null) return false;

                var rental = await _context.Rentals
                    .FirstOrDefaultAsync(r =>
                        r.UserId == userId &&
                        r.MovieId == movieId &&
                        r.Status == RentalStatus.Active);

                var watchHistory = new WatchHistory
                {
                    UserId = userId,
                    MovieId = movieId,
                    RentalId = rental?.Id,
                    WatchedAt = DateTime.UtcNow,
                    WatchDurationSeconds = watchDurationSeconds,
                    TotalDurationSeconds = movie.Duration * 60,
                    CompletionPercentage = (decimal)watchDurationSeconds / (movie.Duration * 60) * 100,
                    IsCompleted = watchDurationSeconds >= (movie.Duration * 60 * 0.9m), 
                    DeviceType = "Web"
                };

                if (watchHistory.IsCompleted)
                {
                    watchHistory.CompletedAt = DateTime.UtcNow;
                }

                _context.WatchHistories.Add(watchHistory);

                if (rental != null)
                {
                    rental.HasWatched = true;
                    rental.LastWatchedAt = DateTime.UtcNow;
                    rental.FirstWatchedAt ??= DateTime.UtcNow;
                    rental.TotalWatchTimeSeconds += watchDurationSeconds;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording watch history");
                return false;
            }
        }

        public async Task<List<WatchHistoryViewModel>> GetWatchHistoryAsync(string userId, int page = 1, int pageSize = 20)
        {
            var history = await _context.WatchHistories
                .Include(wh => wh.Movie)
                    .ThenInclude(m => m!.MovieTranslations)
                .Where(wh => wh.UserId == userId)
                .OrderByDescending(wh => wh.WatchedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return history.Select(wh => new WatchHistoryViewModel
            {
                Id = wh.Id,
                MovieId = wh.MovieId,
                MovieTitle = wh.Movie?.MovieTranslations.FirstOrDefault()?.Title ?? "Unknown",
                MoviePosterUrl = wh.Movie?.PosterImageUrl ?? "",
                WatchedAt = wh.WatchedAt,
                WatchDurationMinutes = wh.WatchDurationSeconds / 60,
                CompletionPercentage = wh.CompletionPercentage,
                IsCompleted = wh.IsCompleted,
                DeviceType = wh.DeviceType ?? "Web",
                WasRented = wh.RentalId.HasValue
            }).ToList();
        }

        public async Task<int> ExpireOldRentalsAsync()
        {
            var expiredRentals = await _context.Rentals
                .Where(r => r.Status == RentalStatus.Active && r.ExpiryDate <= DateTime.UtcNow)
                .ToListAsync();

            foreach (var rental in expiredRentals)
            {
                rental.Status = RentalStatus.Expired;
                rental.IsActive = false;
            }

            await _context.SaveChangesAsync();
            return expiredRentals.Count;
        }

        public async Task<RentalViewModel?> GetRentalByIdAsync(int rentalId, string userId)
        {
            var rental = await _context.Rentals
                .Include(r => r.Movie)
                    .ThenInclude(m => m!.MovieTranslations)
                .Include(r => r.Movie!.Currency)
                .FirstOrDefaultAsync(r => r.Id == rentalId && r.UserId == userId);

            return rental != null ? MapToViewModel(rental) : null;
        }

        private RentalViewModel MapToViewModel(Rental rental)
        {
            var timeRemaining = rental.ExpiryDate - DateTime.UtcNow;
            var translation = rental.Movie?.MovieTranslations.FirstOrDefault();

            return new RentalViewModel
            {
                Id = rental.Id,
                MovieId = rental.MovieId,
                MovieTitle = translation?.Title ?? "Unknown",
                MoviePosterUrl = rental.Movie?.PosterImageUrl ?? "",
                MovieLanguage = rental.Movie?.Language?.IsoCode ?? "",
                RentalDate = rental.RentalDate,
                ExpiryDate = rental.ExpiryDate,
                Price = rental.Price,
                Currency = rental.Movie?.Currency?.Symbol ?? "$",
                Status = rental.Status,
                IsExpired = rental.IsExpired,
                HasWatched = rental.HasWatched,
                DaysRemaining = Math.Max(0, (int)timeRemaining.TotalDays),
                HoursRemaining = Math.Max(0, (int)timeRemaining.TotalHours),
                TimeRemainingText = FormatTimeRemaining(timeRemaining),
                FirstWatchedAt = rental.FirstWatchedAt,
                LastWatchedAt = rental.LastWatchedAt,
                TotalWatchTimeMinutes = rental.TotalWatchTimeSeconds / 60
            };
        }

        private string FormatTimeRemaining(TimeSpan timeRemaining)
        {
            if (timeRemaining.TotalSeconds <= 0) return "Expired";

            if (timeRemaining.TotalDays >= 1)
                return $"{(int)timeRemaining.TotalDays}d {timeRemaining.Hours}h left";

            if (timeRemaining.TotalHours >= 1)
                return $"{(int)timeRemaining.TotalHours}h {timeRemaining.Minutes}m left";

            return $"{timeRemaining.Minutes}m left";
        }
    }
}