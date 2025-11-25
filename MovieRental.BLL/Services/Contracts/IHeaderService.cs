using MovieRental.BLL.ViewModels.Header;

namespace MovieRental.BLL.Services.Contracts
{
    public interface IHeaderService
    {
        Task<HeaderViewModel> GetHeaderAsync();
    }
}
