using Microsoft.AspNetCore.Mvc.Rendering;

namespace MovieRental.BLL.ViewModels.Location
{
    public class LocationUpdateViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
    public class LocationTranslationUpdateViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int LocationID { get; set; }
        public int LanguageId { get; set; }
        public List<SelectListItem>? LanguageList { get; set; }
    }
}