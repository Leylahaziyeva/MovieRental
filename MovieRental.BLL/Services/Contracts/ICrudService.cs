using Microsoft.EntityFrameworkCore.Query;
using MovieRental.DAL.DataContext.Entities;
using System.Linq.Expressions;

namespace MovieRental.BLL.Services.Contracts
{
    public interface ICrudService<TEntity, TViewModel, TCreateViewModel, TUpdateViewModel>
   where TEntity : Entity
    {
        Task<TViewModel?> GetByIdAsync(int id);
        Task<TViewModel> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>,
                                  IIncludableQueryable<TEntity, object>>? include = null, bool AsNoTracking = false);
        Task<IEnumerable<TViewModel>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null,
                                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
                                    Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
                                    bool AsNoTracking = false);
        Task<TViewModel> CreateAsync(TCreateViewModel createViewModel);
        Task<bool> UpdateAsync(int id, TUpdateViewModel model);
        Task<bool> DeleteAsync(int id);

    }
}