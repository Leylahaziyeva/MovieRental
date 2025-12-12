using MovieRental.BLL.ViewModels.Sport;
using MovieRental.BLL.ViewModels.Sport.MovieRental.BLL.ViewModels.Sport;
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
        Task<(IEnumerable<SportViewModel> Sports, int TotalCount)> GetSportsPagedAsync(
            int page = 1,
            int pageSize = 12,
            string? location = null,
            string? category = null);
        Task<SportFilterResultViewModel> GetFilteredSportsAsync(SportFilterViewModel filter);
        Task<SportFilterViewModel> GetFilterOptionsAsync(int languageId);
    }
}