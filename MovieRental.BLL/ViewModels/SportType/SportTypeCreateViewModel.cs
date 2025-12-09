using Microsoft.AspNetCore.Mvc.Rendering;

namespace MovieRental.BLL.ViewModels.SportType
{
    public class SportTypeCreateViewModel
    {
        public required string Name { get; set; }
        public int LanguageId { get; set; }
    }
    public class SportTypeTranslationCreateViewModel
    {
        public required string Name { get; set; }
        public int SportTypeId { get; set; }
        public int LanguageId { get; set; }
        public List<SelectListItem>? LanguageList { get; set; }
    }
}