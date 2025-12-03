using Microsoft.EntityFrameworkCore.Query;
using MovieRental.DAL.DataContext.Entities;
using System.Linq.Expressions;

public interface IRepositoryAsync<T> where T : Entity
{
    Task<T?> GetByIdAsync(int id);

    Task<T?> GetAsync(
        Expression<Func<T, bool>> predicate,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool AsNoTracking = false);

    Task<IList<T>> GetAllAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        bool AsNoTracking = false);

    Task<(IList<T> Items, int TotalCount)> GetPagedAsync(
        Expression<Func<T, bool>>? predicate = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
        int page = 1,
        int pageSize = 10,
        bool AsNoTracking = false);

    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<T> DeleteAsync(T entity);
}