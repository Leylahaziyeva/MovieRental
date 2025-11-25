using MovieRental.BLL.ViewModels.Language;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Contracts
{
    public interface ILanguageService : ICrudService<Language, LanguageViewModel, LanguageCreateViewModel, LanguageUpdateViewModel>
    {
    }
}