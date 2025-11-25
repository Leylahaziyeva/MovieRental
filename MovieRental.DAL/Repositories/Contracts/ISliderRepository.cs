using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.DAL.Repositories.Contracts
{
    public interface ISliderRepository : IRepositoryAsync<Slider>
    {
        Task<List<Slider>> GetAllActiveSlidersAsync();
        Task<Slider?> GetSliderWithTranslationsAsync(int id);
    }
}