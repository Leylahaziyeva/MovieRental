using MovieRental.BLL.ViewModels.Genre;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Contracts
{
    namespace MovieRental.BLL.Services.Contracts
    {
        public interface IGenreService : ICrudService<Genre, GenreViewModel, GenreCreateViewModel, GenreUpdateViewModel>
        {
   
        }
    }
}