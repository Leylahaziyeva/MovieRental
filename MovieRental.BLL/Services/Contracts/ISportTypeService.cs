using MovieRental.BLL.ViewModels.SportType;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Contracts
{
    public interface ISportTypeService : ICrudService<SportType, SportTypeViewModel, SportTypeCreateViewModel, SportTypeUpdateViewModel>
    {
        Task<IEnumerable<SportTypeViewModel>> GetActiveSportTypesAsync();
        Task<SportTypeViewModel?> GetSportTypeWithTranslationAsync(int id, int languageId);
        Task<IEnumerable<SportTypeViewModel>> GetSportTypesWithCountAsync(int languageId);
    }
}