namespace MovieRental.BLL.Services.Contracts
{
    public interface ISearchHistoryService
    {
        Task SaveSearchAsync(string query, string? type = null);
        Task<List<string>> GetUserRecentSearchesAsync(int limit = 5);
        Task<List<string>> GetPopularSearchesAsync(int limit = 10);
        Task ClearUserHistoryAsync();
    }
}