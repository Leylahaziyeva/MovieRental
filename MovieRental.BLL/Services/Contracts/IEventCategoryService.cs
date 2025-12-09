using MovieRental.BLL.ViewModels.EventCategory;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.Services.Contracts
{
    public interface IEventCategoryService : ICrudService<EventCategory, EventCategoryViewModel, EventCategoryCreateViewModel, EventCategoryUpdateViewModel>
    {
        Task<IEnumerable<EventCategoryOption>> GetCategoriesForFilterAsync(int languageId);
        Task<string?> GetCategoryNameAsync(int categoryId, int languageId);
    }
}
