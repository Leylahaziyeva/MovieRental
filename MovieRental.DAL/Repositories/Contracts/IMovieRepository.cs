using Microsoft.EntityFrameworkCore.Query;
using MovieRental.DAL.DataContext.Entities;
using System.Linq.Expressions;

namespace MovieRental.DAL.Repositories.Contracts
{
    public interface IMovieRepository : IRepositoryAsync<Movie>
    {
        IQueryable<Movie> GetQuery();
        Task<IList<Movie>> GetFilteredMoviesAsync(Expression<Func<Movie, bool>> predicate,
            Func<IQueryable<Movie>, IIncludableQueryable<Movie, object>>? include = null,
            Func<IQueryable<Movie>, IOrderedQueryable<Movie>>? orderBy = null, int? take = null);

        Task<Movie?> GetMovieWithDetailsAsync(int movieId, int languageId);
    }
}