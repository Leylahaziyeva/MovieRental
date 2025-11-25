using Microsoft.EntityFrameworkCore;
using MovieRental.DAL.DataContext;
using MovieRental.DAL.DataContext.Entities;
using MovieRental.DAL.Repositories.Contracts;

namespace MovieRental.DAL.Repositories
{
    public class SliderRepository : EfCoreRepository<Slider>, ISliderRepository
    {
        private readonly AppDbContext _dbContext;

        public SliderRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<List<Slider>> GetAllActiveSlidersAsync()
        {
            return _dbContext.Sliders
                .Where(x => x.IsActive)
                .Include(x => x.SliderTranslations)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();
        }

        public Task<Slider?> GetSliderWithTranslationsAsync(int id)
        {
            return _dbContext.Sliders
                .Include(x => x.SliderTranslations)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
    }
}