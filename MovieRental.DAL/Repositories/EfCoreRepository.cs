using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MovieRental.DAL.DataContext;
using MovieRental.DAL.DataContext.Entities;
using MovieRental.DAL.Repositories.Contracts;
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

        public async virtual Task<T> AddAsync(T entity)
        {
            var entityEntry = await DbContext.Set<T>().AddAsync(entity);
            await DbContext.SaveChangesAsync();

            return entityEntry.Entity;
        }


        public async virtual Task<T> DeleteAsync(T entity)
        {
            var entityEntry = DbContext.Set<T>().Remove(entity);
            await DbContext.SaveChangesAsync();

            return entityEntry.Entity;
        }

        public async virtual Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, bool AsNoTracking = false)
        {

            IQueryable<T> queryable = DbContext.Set<T>();

            if (AsNoTracking) queryable = queryable.AsNoTracking();

            if (include != null) queryable = include(queryable);

            if (predicate != null) queryable = queryable.Where(predicate);

            if (orderBy != null) queryable = orderBy(queryable);


            return await queryable.ToListAsync();
        }

        public async virtual Task<T?> GetAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, bool AsNoTracking = false)
        {
            var query = DbContext.Set<T>().AsQueryable();

            if (AsNoTracking) query = query.AsNoTracking();
            if (include != null) query = include(query);

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async virtual Task<T?> GetByIdAsync(int id)
        {
            return await DbContext.Set<T>().FindAsync(id);
        }

        public async virtual Task<T> UpdateAsync(T entity)
        {
            var entityEntry = DbContext.Set<T>().Update(entity);
            await DbContext.SaveChangesAsync();

            return entityEntry.Entity;
        }
    }
}
