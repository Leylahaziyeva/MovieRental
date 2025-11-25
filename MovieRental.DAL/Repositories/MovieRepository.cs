using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MovieRental.DAL.DataContext;
using MovieRental.DAL.DataContext.Entities;
using MovieRental.DAL.Repositories.Contracts;
using System.Linq.Expressions;

namespace MovieRental.DAL.Repositories
{
    public class MovieRepository : EfCoreRepository<Movie>, IMovieRepository
    {
        public MovieRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IList<Movie>> GetFilteredMoviesAsync(
            Expression<Func<Movie, bool>> predicate,
            Func<IQueryable<Movie>, IIncludableQueryable<Movie, object>>? include = null,
            Func<IQueryable<Movie>, IOrderedQueryable<Movie>>? orderBy = null,
            int? take = null)
        {
            IQueryable<Movie> query = DbContext.Set<Movie>();

            if (include != null)
                query = include(query);

            query = query.Where(predicate);

            if (orderBy != null)
                query = orderBy(query);

            if (take.HasValue)
                query = query.Take(take.Value);

            return await query.ToListAsync();
        }

        public async Task<Movie?> GetMovieWithDetailsAsync(int movieId, int languageId)
        {
            return await DbContext.Movies
                .Where(m => m.Id == movieId && !m.IsDeleted)
                .Include(m => m.MovieTranslations.Where(mt => mt.LanguageId == languageId))
                .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                    .ThenInclude(g => g.GenreTranslations.Where(gt => gt.LanguageId == languageId))
                .Include(m => m.MovieActors)
                    .ThenInclude(ma => ma.Actor)
                    .ThenInclude(a => a.ActorTranslations.Where(at => at.LanguageId == languageId))
                .Include(m => m.MovieImages)
                .Include(m => m.MovieVideos)
                    .ThenInclude(mv => mv.MovieVideoTranslations.Where(mvt => mvt.LanguageId == languageId))
                .Include(m => m.MovieSocialLinks)
                .FirstOrDefaultAsync();
        }

        public IQueryable<Movie> GetQuery()
        {
            return DbContext.Movies.AsQueryable();
        }
    }
}