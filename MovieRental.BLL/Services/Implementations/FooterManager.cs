using MovieRental.BLL.Services.Contracts;
using MovieRental.BLL.ViewModels.Footer;

namespace MovieRental.BLL.Services.Implementations
{
    public class FooterManager : IFooterService
    {
        public Task<FooterViewModel> GetFooterAsync()
        {
            var viewModel = new FooterViewModel();

            return Task.FromResult(viewModel);
        }
    }
}
