using MovieRental.BLL.ViewModels.Language;

namespace MovieRental.BLL.ViewModels.Genre
{
    public class GenreViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public List<GenreTranslationViewModel> GenreTranslations { get; set; } = [];
    }

    public class GenreTranslationViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public LanguageViewModel? Language { get; set; }
    }
}
