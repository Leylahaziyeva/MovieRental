using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.Services.Implementations;
using MovieRental.BLL.ViewModels.Account;
using MovieRental.BLL.ViewModels.UserList;
using System.Security.Claims;

namespace MovieRental.MVC.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IWatchlistService _watchlistService;
        private readonly IUserListService _userListService;
        private readonly StringLocalizerManager _localizer;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(
            IAccountService accountService,
            IWatchlistService watchlistService,
            IUserListService userListService,
            StringLocalizerManager localizer,
            ILogger<ProfileController> logger)
        {
            _accountService = accountService;
            _watchlistService = watchlistService;
            _userListService = userListService;
            _localizer = localizer;
            _logger = logger;
        }

        #region Profile

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var profile = await _accountService.GetUserProfileAsync(userId);

            if (profile == null)
            {
                return NotFound();
            }

            return View(profile);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(AccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            if (model.ProfileImageFile != null)
            {
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/user");

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid() + Path.GetExtension(model.ProfileImageFile.FileName);
                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImageFile.CopyToAsync(stream);
                }

                model.ProfileImagePath = fileName;
            }

            var result = await _accountService.UpdateProfileAsync(userId, model);

            if (result.Succeeded)
            {
                TempData["Success"] = _localizer.GetValue("ProfileUpdatedSuccessfully");
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Index", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = _localizer.GetValue("PleaseFillAllFields");
                return RedirectToAction(nameof(Index));
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var result = await _accountService.ChangePasswordAsync(userId, model);

            if (result.Succeeded)
            {
                TempData["PasswordSuccess"] = _localizer.GetValue("PasswordChangedSuccessfully");
            }
            else
            {
                TempData["Error"] = result.Errors.FirstOrDefault()?.Description ?? _localizer.GetValue("ErrorOccurred");
            }

            return RedirectToAction(nameof(Index));
        }

        #endregion

        #region Watchlist

        [HttpGet]
        public async Task<IActionResult> LoadMoreWatchlist(int page = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var watchlist = await _watchlistService.GetUserWatchlistAsync(userId, page, pageSize: 12);

            return PartialView("_WatchlistPartial", watchlist.Movies);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToWatchlist(int movieId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = _localizer.GetValue("PleaseLoginFirst") });
            }

            try
            {
                var result = await _watchlistService.AddToWatchlistAsync(userId, movieId);

                if (result)
                {
                    _logger.LogInformation($"User {userId} added movie {movieId} to watchlist");
                    return Json(new { success = true, message = _localizer.GetValue("AddedToWatchlist") });
                }

                return Json(new { success = false, message = _localizer.GetValue("AlreadyInWatchlist") });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding movie {movieId} to watchlist for user {userId}");
                return Json(new { success = false, message = _localizer.GetValue("ErrorOccurred") });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromWatchlist(int movieId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = _localizer.GetValue("PleaseLoginFirst") });
            }

            try
            {
                var result = await _watchlistService.RemoveFromWatchlistAsync(userId, movieId);

                if (result)
                {
                    _logger.LogInformation($"User {userId} removed movie {movieId} from watchlist");
                    return Json(new { success = true, message = _localizer.GetValue("RemovedFromWatchlist") });
                }

                return Json(new { success = false, message = _localizer.GetValue("NotInWatchlist") });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing movie {movieId} from watchlist for user {userId}");
                return Json(new { success = false, message = _localizer.GetValue("ErrorOccurred") });
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckWatchlistStatus(int movieId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { isInWatchlist = false });
            }

            var isInWatchlist = await _watchlistService.IsInWatchlistAsync(userId, movieId);

            return Json(new { isInWatchlist });
        }

        #endregion

        #region User Lists

        [HttpGet]
        public async Task<IActionResult> LoadMoreUserLists(int page = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var userLists = await _userListService.GetUserListsAsync(userId, page, pageSize: 12);

            return PartialView("_UserListsPartial", userLists.Lists);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateList(CreateUserListViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = _localizer.GetValue("PleaseFillAllFields") });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = _localizer.GetValue("PleaseLoginFirst") });
            }

            try
            {
                var listId = await _userListService.CreateUserListAsync(userId, model);

                _logger.LogInformation($"User {userId} created list {listId}");
                return Json(new { success = true, listId, message = _localizer.GetValue("ListCreatedSuccessfully") });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating list for user {userId}");
                return Json(new { success = false, message = _localizer.GetValue("ErrorOccurred") });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateList(int listId, CreateUserListViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = _localizer.GetValue("PleaseFillAllFields") });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = _localizer.GetValue("PleaseLoginFirst") });
            }

            try
            {
                var result = await _userListService.UpdateUserListAsync(listId, userId, model);

                if (result)
                {
                    _logger.LogInformation($"User {userId} updated list {listId}");
                    return Json(new { success = true, message = _localizer.GetValue("ListUpdatedSuccessfully") });
                }

                return Json(new { success = false, message = _localizer.GetValue("ListNotFound") });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating list {listId} for user {userId}");
                return Json(new { success = false, message = _localizer.GetValue("ErrorOccurred") });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteList(int listId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = _localizer.GetValue("PleaseLoginFirst") });
            }

            try
            {
                var result = await _userListService.DeleteUserListAsync(listId, userId);

                if (result)
                {
                    _logger.LogInformation($"User {userId} deleted list {listId}");
                    return Json(new { success = true, message = _localizer.GetValue("ListDeletedSuccessfully") });
                }

                return Json(new { success = false, message = _localizer.GetValue("ListNotFound") });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting list {listId} for user {userId}");
                return Json(new { success = false, message = _localizer.GetValue("ErrorOccurred") });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMovieToList(int listId, int movieId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = _localizer.GetValue("PleaseLoginFirst") });
            }

            try
            {
                var result = await _userListService.AddMovieToListAsync(listId, movieId, userId);

                if (result)
                {
                    _logger.LogInformation($"User {userId} added movie {movieId} to list {listId}");
                    return Json(new { success = true, message = _localizer.GetValue("MovieAddedToList") });
                }

                return Json(new { success = false, message = _localizer.GetValue("MovieAlreadyInList") });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding movie {movieId} to list {listId} for user {userId}");
                return Json(new { success = false, message = _localizer.GetValue("ErrorOccurred") });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveMovieFromList(int listId, int movieId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = _localizer.GetValue("PleaseLoginFirst") });
            }

            try
            {
                var result = await _userListService.RemoveMovieFromListAsync(listId, movieId, userId);

                if (result)
                {
                    _logger.LogInformation($"User {userId} removed movie {movieId} from list {listId}");
                    return Json(new { success = true, message = _localizer.GetValue("MovieRemovedFromList") });
                }

                return Json(new { success = false, message = _localizer.GetValue("MovieNotInList") });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error removing movie {movieId} from list {listId} for user {userId}");
                return Json(new { success = false, message = _localizer.GetValue("ErrorOccurred") });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetListDetails(int listId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = _localizer.GetValue("PleaseLoginFirst") });
            }

            try
            {
                var list = await _userListService.GetUserListByIdAsync(listId, userId);

                if (list == null)
                {
                    return Json(new { success = false, message = _localizer.GetValue("ListNotFound") });
                }

                return Json(new { success = true, list });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting list {listId} details for user {userId}");
                return Json(new { success = false, message = _localizer.GetValue("ErrorOccurred") });
            }
        }

        #endregion

        #region List Details

        [HttpGet]
        public async Task<IActionResult> ListDetails(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var list = await _userListService.GetUserListByIdAsync(id, userId);

            if (list == null)
            {
                TempData["Error"] = _localizer.GetValue("ListNotFound");
                return RedirectToAction(nameof(Index));
            }

            return View(list);
        }

        #endregion
    }
}