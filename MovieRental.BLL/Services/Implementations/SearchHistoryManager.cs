using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MovieRental.BLL.Services.Contracts;
using MovieRental.DAL.DataContext;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Implementations
{
    public class SearchHistoryManager : ISearchHistoryService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppUser> _userManager;

        public SearchHistoryManager(
            AppDbContext context,
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task SaveSearchAsync(string query, string? type = null)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
                return;

            if (string.IsNullOrWhiteSpace(query))
                return;

            var userId = _userManager.GetUserId(httpContext.User);
            if (string.IsNullOrEmpty(userId))
                return;

            var recentSearch = await _context.SearchHistories
                .Where(sh => sh.UserId == userId &&
                            sh.SearchQuery == query &&
                            sh.SearchDate > DateTime.UtcNow.AddMinutes(-5) &&
                            !sh.IsDeleted)
                .FirstOrDefaultAsync();

            if (recentSearch != null)
                return; 

            var searchHistory = new SearchHistory
            {
                UserId = userId,
                SearchQuery = query.Trim(),
                SearchType = type,
                SearchDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false,
                CreatedBy = httpContext.User.Identity?.Name ?? "System",
                UpdatedBy = httpContext.User.Identity?.Name ?? "System"
            };

            _context.SearchHistories.Add(searchHistory);
            await _context.SaveChangesAsync();
        }

        public async Task<List<string>> GetUserRecentSearchesAsync(int limit = 5)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
                return new List<string>();

            var userId = _userManager.GetUserId(httpContext.User);
            if (string.IsNullOrEmpty(userId))
                return new List<string>();

            return await _context.SearchHistories
                .Where(sh => sh.UserId == userId && !sh.IsDeleted)
                .OrderByDescending(sh => sh.SearchDate)
                .Select(sh => sh.SearchQuery)
                .Distinct()
                .Take(limit)
                .ToListAsync();
        }

        public async Task<List<string>> GetPopularSearchesAsync(int limit = 10)
        {
            return await _context.SearchHistories
                .Where(sh => !sh.IsDeleted && sh.SearchDate > DateTime.UtcNow.AddDays(-30))
                .GroupBy(sh => sh.SearchQuery)
                .OrderByDescending(g => g.Count())
                .Take(limit)
                .Select(g => g.Key)
                .ToListAsync();
        }

        public async Task ClearUserHistoryAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
                return;

            var userId = _userManager.GetUserId(httpContext.User);
            if (string.IsNullOrEmpty(userId))
                return;

            var userSearches = await _context.SearchHistories
                .Where(sh => sh.UserId == userId && !sh.IsDeleted)
                .ToListAsync();

            foreach (var search in userSearches)
            {
                search.IsDeleted = true;
                search.UpdatedAt = DateTime.UtcNow;
                search.UpdatedBy = httpContext.User.Identity?.Name ?? "System";
            }

            await _context.SaveChangesAsync();
        }
    }
}