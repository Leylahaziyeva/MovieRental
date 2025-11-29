using Microsoft.AspNetCore.Mvc.Rendering;

namespace MovieRental.BLL.ViewModels.Genre
{
    public class GenreUpdateViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<GenreTranslationUpdateViewModel> Translations { get; set; } = [];
        public List<SelectListItem>? Languages { get; set; }
    }

    public class GenreTranslationUpdateViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int GenreId { get; set; }
        public int LanguageId { get; set; }
        public List<SelectListItem>? Languages { get; set; }
    }
}
