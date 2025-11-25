using MovieRental.BLL.ViewModels;

namespace MovieRental.BLL.Services.Contracts
{
    public interface IHomeService
    {
        Task<HomeViewModel> GetHomeViewModelAsync();
    }
}
