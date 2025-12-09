using Microsoft.AspNetCore.Mvc.Rendering;

namespace MovieRental.BLL.ViewModels.EventCategory
{
    public class EventCategoryCreateViewModel
    {
        public required string Name { get; set; }
    }
    public class EventCategoryTranslationCreateViewModel
    {
        public required string Name { get; set; }
        public int EventCategoryId { get; set; }
        public int LanguageId { get; set; }
        public List<SelectListItem>? LanguageList { get; set; }
    }
}