using Microsoft.AspNetCore.Mvc.Rendering;

namespace MovieRental.BLL.ViewModels.Genre
{
    public class GenreUpdateViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int DefaultLanguageId { get; set; } = 1;  

        public List<GenreTranslationUpdateViewModel> Translations { get; set; } = [];
        public List<SelectListItem>? Languages { get; set; }
    }

    public class GenreTranslationUpdateViewModel
    {
        public int Id { get; set; }  
        public required string Name { get; set; }
        public int LanguageId { get; set; }
    }
}
