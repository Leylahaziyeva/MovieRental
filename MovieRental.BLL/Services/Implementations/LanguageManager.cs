using AutoMapper;
using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Language;
using MovieRental.DAL.DataContext.Entities;
using MovieRental.DAL.Repositories.Contracts;

namespace MovieRental.BLL.Services.Implementations
{
    public class LanguageManager : CrudManager<Language, LanguageViewModel, LanguageCreateViewModel, LanguageUpdateViewModel>, ILanguageService
    {
        public LanguageManager(IRepositoryAsync<Language> repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}