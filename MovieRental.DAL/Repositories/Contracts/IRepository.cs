using Microsoft.EntityFrameworkCore.Query;
using MovieRental.DAL.DataContext.Entities;
using System.Linq.Expressions;

namespace MovieRental.DAL.Repositories.Contracts
{
    public interface IRepositoryAsync<T> where T : Entity
    {
        Task<T?> GetByIdAsync(int id);

        Task<T?> GetAsync(Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            bool AsNoTracking = false);

        //Task<Paginate<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null,
        //                                Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        //                                Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        //                                int index = 0, int size = 10, bool enableTracking = false);

        Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null,
                                        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
                                        bool AsNoTracking = false);

        Task<T> AddAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<T> DeleteAsync(T entity);
    }
}
