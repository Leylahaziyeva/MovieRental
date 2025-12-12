using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Rental;
using System.Security.Claims;

namespace MovieRental.MVC.Controllers
{
    [Authorize]
    public class RentalController : Controller
    {
        private readonly IRentalService _rentalService;
        private readonly ILogger<RentalController> _logger;

        public RentalController(
            IRentalService rentalService,
            ILogger<RentalController> logger)
        {
            _rentalService = rentalService;
            _logger = logger;
        }

        // ✅ POST: Rent Movie
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RentMovie(int movieId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Please login first" });
            }

            var result = await _rentalService.RentMovieAsync(userId, movieId);

            if (result.Success)
            {
                _logger.LogInformation($"User {userId} successfully rented movie {movieId}");
            }

            return Json(new
            {
                success = result.Success,
                message = result.Message,
                rentalId = result.RentalId,
                expiryDate = result.ExpiryDate?.ToString("yyyy-MM-dd HH:mm"),
                price = result.Price
            });
        }

        // ✅ POST: Cancel Rental
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelRental(int rentalId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var result = await _rentalService.CancelRentalAsync(rentalId, userId);

            return Json(new
            {
                success = result,
                message = result ? "Rental cancelled successfully" : "Failed to cancel rental"
            });
        }

        // ✅ GET: Check if movie is rented
        [HttpGet]
        public async Task<IActionResult> CheckRentalStatus(int movieId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { isRented = false });
            }

            var hasRental = await _rentalService.HasActiveRentalAsync(userId, movieId);
            var rental = hasRental ? await _rentalService.GetActiveRentalForMovieAsync(userId, movieId) : null;

            return Json(new
            {
                isRented = hasRental,
                rental = rental
            });
        }

        // ✅ POST: Record Watch
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecordWatch(int movieId, int watchDurationSeconds)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "Unauthorized" });
            }

            var result = await _rentalService.RecordWatchAsync(userId, movieId, watchDurationSeconds);

            return Json(new { success = result });
        }

        // ✅ GET: Active Rentals Partial View
        [HttpGet]
        public async Task<IActionResult> GetActiveRentals()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return PartialView("_ActiveRentalsPartial", new List<RentalViewModel>());
            }

            var rentals = await _rentalService.GetUserActiveRentalsAsync(userId);
            return PartialView("_ActiveRentalsPartial", rentals);
        }

        // ✅ GET: Watch History Partial View
        [HttpGet]
        public async Task<IActionResult> GetWatchHistory(int page = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return PartialView("_WatchHistoryPartial", new List<WatchHistoryViewModel>());
            }

            var history = await _rentalService.GetWatchHistoryAsync(userId, page, 12);
            return PartialView("_WatchHistoryPartial", history);
        }
    }
}