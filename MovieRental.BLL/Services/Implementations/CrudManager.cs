using AutoMapper;
using Microsoft.EntityFrameworkCore.Query;
using MovieRental.BLL.Services.Contracts;
using MovieRental.DAL.DataContext.Entities;
using MovieRental.DAL.Repositories.Contracts;
using System.Linq.Expressions;

namespace MovieRental.BLL.Services.Implementations
{
    public class CrudManager<TEntity, TViewModel, TCreateViewModel, TUpdateViewModel> : ICrudService<TEntity, TViewModel, TCreateViewModel, TUpdateViewModel>
  where TEntity : Entity
    {
        protected readonly IRepositoryAsync<TEntity> Repository;
        protected readonly IMapper Mapper;

        public CrudManager(IRepositoryAsync<TEntity> repository, IMapper mapper)
        {
            Repository = repository;
            Mapper = mapper;
        }

        public virtual async Task<TViewModel> CreateAsync(TCreateViewModel createViewModel)
        {
            var entity = Mapper.Map<TEntity>(createViewModel);
            var createdEntity = await Repository.AddAsync(entity);

            return Mapper.Map<TViewModel>(createdEntity);
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await Repository.GetByIdAsync(id);

            if (entity == null)
                return false;

            await Repository.DeleteAsync(entity);

            return true;
        }

        public async virtual Task<IEnumerable<TViewModel>> GetAllAsync(Expression<Func<TEntity, bool>>? predicate = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool AsNoTracking = false)
        {
            var entities = await Repository.GetAllAsync(predicate, orderBy, include, AsNoTracking);

            var viewModels = Mapper.Map<IEnumerable<TViewModel>>(entities);

            return viewModels;
        }

        public async virtual Task<TViewModel> GetAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, bool AsNoTracking = false)
        {
            var entity = await Repository.GetAsync(predicate, include);
            var viewModel = Mapper.Map<TViewModel>(entity);

            return viewModel;
        }

        public async virtual Task<TViewModel?> GetByIdAsync(int id)
        {
            var entity = await Repository.GetByIdAsync(id);

            if (entity == null)
                return default;

            var viewModel = Mapper.Map<TViewModel>(entity);

            return viewModel;
        }

        public async virtual Task<bool> UpdateAsync(int id, TUpdateViewModel model)
        {
            var entity = await Repository.GetByIdAsync(id);

            if (entity == null)
                return false;

            Mapper.Map(model, entity);

            await Repository.UpdateAsync(entity);

            return true;
        }
    }
}