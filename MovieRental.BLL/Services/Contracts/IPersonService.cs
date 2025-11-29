using MovieRental.BLL.ViewModels.Person;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Contracts
{
    public interface IPersonService : ICrudService<Person, PersonViewModel, PersonCreateViewModel, PersonUpdateViewModel>
    {
        Task<IEnumerable<PersonViewModel>> GetByPersonTypeAsync(PersonType personType);
        Task<IEnumerable<PersonViewModel>> GetTopPersonsAsync(int count = 10);
        Task<IEnumerable<PersonViewModel>> GetSportsmenAsync();
        Task<IEnumerable<PersonViewModel>> GetActorsAsync();
        Task<IEnumerable<PersonViewModel>> GetArtistsAsync();
    }
}