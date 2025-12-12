using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MovieRental.DAL.DataContext;
using MovieRental.DAL.DataContext.Entities;
using System.Linq.Expressions;

namespace MovieRental.DAL.Repositories
{
    public class EfCoreRepository<T> : IRepositoryAsync<T> where T : Entity
    {
        protected readonly AppDbContext DbContext;

        public EfCoreRepository(AppDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            var entityEntry = await DbContext.Set<T>().AddAsync(entity);
            await DbContext.SaveChangesAsync();
            return entityEntry.Entity;
        }

        public virtual async Task<T> DeleteAsync(T entity)
        {
            var entityEntry = DbContext.Set<T>().Remove(entity);
            await DbContext.SaveChangesAsync();
            return entityEntry.Entity;
        }

        public virtual async Task<IList<T>> GetAllAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            bool AsNoTracking = false)
        {
            IQueryable<T> queryable = DbContext.Set<T>();

            if (AsNoTracking)
                queryable = queryable.AsNoTracking();

            if (include != null)
                queryable = include(queryable);

            if (predicate != null)
                queryable = queryable.Where(predicate);

            if (orderBy != null)
                queryable = orderBy(queryable);

            return await queryable.ToListAsync();
        }

        public virtual async Task<T?> GetAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            bool AsNoTracking = false)
        {
            var query = DbContext.Set<T>().AsQueryable();

            if (AsNoTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync(predicate);
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await DbContext.Set<T>().FindAsync(id);
        }

        public virtual async Task<(IList<T> Items, int TotalCount)> GetPagedAsync(
            Expression<Func<T, bool>>? predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            int page = 1,
            int pageSize = 10,
            bool AsNoTracking = false)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Maximum limit to prevent abuse

            IQueryable<T> query = DbContext.Set<T>();

            if (AsNoTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            var totalCount = await query.CountAsync();

            if (orderBy != null)
                query = orderBy(query);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            var entityEntry = DbContext.Set<T>().Update(entity);
            await DbContext.SaveChangesAsync();
            return entityEntry.Entity;
        }
    }
}