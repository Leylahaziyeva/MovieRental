using MovieRental.DAL.DataContext;
using MovieRental.DAL.DataContext.Entities;
using MovieRental.DAL.Repositories.Contracts;

namespace MovieRental.DAL.Repositories
{
    public class LanguageRepository : EfCoreRepository<Language>, ILanguageRepository
    {
        public LanguageRepository(AppDbContext context) : base(context)
        {
        }
    }

}
