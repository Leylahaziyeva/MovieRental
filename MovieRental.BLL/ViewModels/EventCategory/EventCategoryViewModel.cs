using Microsoft.AspNetCore.Mvc.Rendering;
using MovieRental.BLL.ViewModels.Language;

namespace MovieRental.BLL.ViewModels.EventCategory
{
    public class EventCategoryViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int EventCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class EventCategoryTranslationViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int EventCategoryId { get; set; }
        public int LanguageId { get; set; }
        public LanguageViewModel? Language { get; set; }
    }
}