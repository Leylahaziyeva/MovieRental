using Microsoft.AspNetCore.Mvc.Rendering;

namespace MovieRental.BLL.ViewModels.Genre
{
    public class GenreCreateViewModel
    {
        public required string Name { get; set; }
        public int DefaultLanguageId { get; set; } = 1;  

        public List<GenreTranslationCreateViewModel> Translations { get; set; } = [];
        public List<SelectListItem>? Languages { get; set; }
    }

    public class GenreTranslationCreateViewModel
    {
        public required string Name { get; set; }
        public int LanguageId { get; set; }
    }
}
