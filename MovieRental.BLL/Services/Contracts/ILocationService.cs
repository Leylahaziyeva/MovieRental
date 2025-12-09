using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Location;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Implementations
{
    public interface ILocationService : ICrudService<Location, LocationViewModel, LocationCreateViewModel, LocationUpdateViewModel>
    {
        Task<IEnumerable<LocationOption>> GetLocationsForFilterAsync(int languageId);
        Task<string?> GetLocationNameAsync(int locationId, int languageId);
    }
}