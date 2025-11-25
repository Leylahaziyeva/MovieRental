using MovieRental.BLL.ViewModels.Footer;

namespace MovieRental.BLL.Services.Contracts
{
    public interface IFooterService
    {
        Task<FooterViewModel> GetFooterAsync();
    }
}
