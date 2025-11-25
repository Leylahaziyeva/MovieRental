using MovieRental.BLL.ViewModels;
using MovieRental.BLL.ViewModels.Slider;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Contracts
{
    public interface ISliderService : ICrudService<Slider, SliderViewModel, SliderCreateViewModel, SliderUpdateViewModel>
    {
        Task<SliderViewModel?> GetSliderByIdAsync(int id, int languageId);
        Task<List<SliderViewModel>> GetAllActiveSlidersAsync(int languageId);
    }
}