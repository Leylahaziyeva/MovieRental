using MovieRental.BLL.ViewModels.Person;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Contracts
{
    public interface IPersonTranslationService : ICrudService<PersonTranslation, PersonTranslationViewModel, PersonTranslationCreateViewModel, PersonTranslationUpdateViewModel>
    {
        Task<IEnumerable<PersonTranslationViewModel>> GetAllByPersonIdAsync(int personId);
        Task<PersonTranslationViewModel?> GetByPersonAndLanguageAsync(int personId, int languageId);
        Task<bool> ExistsAsync(int personId, int languageId);
    }
}