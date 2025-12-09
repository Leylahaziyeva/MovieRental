using Microsoft.AspNetCore.Mvc.Rendering;

namespace MovieRental.BLL.ViewModels.Location
{
    public class LocationCreateViewModel
    {
        public required string Name { get; set; }
        public int LanguageId { get; set; }
    }
    public class LocationTranslationCreateViewModel
    {
        public required string Name { get; set; }
        public int LocationId { get; set; }
        public int LanguageId { get; set; }
        public List<SelectListItem>? LanguageList { get; set; }
    }
}