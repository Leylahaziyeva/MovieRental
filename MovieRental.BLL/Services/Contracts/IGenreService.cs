using MovieRental.BLL.ViewModels.Genre;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Contracts
{
    public interface IGenreService : ICrudService<Genre, GenreViewModel, GenreCreateViewModel, GenreUpdateViewModel>
    {
        Task<IEnumerable<GenreViewModel>> GetAllActiveAsync(int languageId);
        Task<GenreViewModel?> GetByIdWithLanguageAsync(int id, int languageId);  
        Task<IEnumerable<GenreViewModel>> SearchByNameAsync(string searchTerm, int languageId);
        Task<IEnumerable<GenreViewModel>> GetPopularGenresAsync(int languageId, int count = 10);

        Task<bool> TranslationExistsAsync(int genreId, int languageId);
        Task<bool> AddTranslationAsync(int genreId, GenreTranslationCreateViewModel translation);
        Task<bool> UpdateTranslationAsync(int translationId, GenreTranslationUpdateViewModel translation);
        Task<bool> DeleteTranslationAsync(int translationId);

        Task<int> GetGenreMovieCountAsync(int genreId); 
        Task<int> GetTotalGenresCountAsync();  
    }
}