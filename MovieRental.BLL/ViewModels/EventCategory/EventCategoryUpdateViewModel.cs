using Microsoft.AspNetCore.Mvc.Rendering;

namespace MovieRental.BLL.ViewModels.EventCategory
{
    public class EventCategoryUpdateViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
    public class EventCategoryTranslationUpdateViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int EventCategoryId { get; set; }
        public int LanguageId { get; set; }
        public List<SelectListItem>? LanguageList { get; set; }
    }
}