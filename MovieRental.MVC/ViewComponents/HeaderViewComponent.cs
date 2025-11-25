using Microsoft.AspNetCore.Mvc;
using MovieRental.BLL.Services.Contracts;

namespace MovieRental.MVC.ViewComponents
{
    public class HeaderViewComponent : ViewComponent
    {
        private readonly IHeaderService _headerService;

        public HeaderViewComponent(IHeaderService headerService)
        {
            _headerService = headerService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var model = await _headerService.GetHeaderAsync();
            return View(model);
        }
    }
}
