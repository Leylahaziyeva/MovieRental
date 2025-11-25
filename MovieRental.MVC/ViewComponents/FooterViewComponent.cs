using Microsoft.AspNetCore.Mvc;
using MovieRental.BLL.Services.Contracts;

namespace MovieRental.MVC.ViewComponents
{
    public class FooterViewComponent : ViewComponent
    {
        private readonly IFooterService _footerService;

        public FooterViewComponent(IFooterService footerService)
        {
            _footerService = footerService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _footerService.GetFooterAsync();
            return View(model);
        }
    }
}
