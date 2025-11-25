using MovieRental.BLL.ViewModels.Sport;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Contracts
{
    public interface ISportService : ICrudService<Sport, SportViewModel, SportCreateViewModel, SportUpdateViewModel>
    {
        Task<IEnumerable<SportViewModel>> GetUpcomingSportsAsync(int languageId);
        Task<SportViewModel?> GetSportByIdWithTranslationsAsync(int id, int languageId);
        Task<SportFilterResultViewModel> GetFilteredSportsAsync(SportFilterViewModel filter);
    }
}