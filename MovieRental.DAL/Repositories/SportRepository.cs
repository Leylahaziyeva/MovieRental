using MovieRental.DAL.DataContext;
using MovieRental.DAL.DataContext.Entities;
using MovieRental.DAL.Repositories.Contracts;

namespace MovieRental.DAL.Repositories
{
    public class SportRepository : EfCoreRepository<Sport>, ISportRepository
    {
        public SportRepository(AppDbContext context) : base(context)
        {
            
        }
    }
}
