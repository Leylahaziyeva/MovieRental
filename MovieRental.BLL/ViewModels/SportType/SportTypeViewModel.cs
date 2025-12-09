using MovieRental.BLL.ViewModels.Language;

namespace MovieRental.BLL.ViewModels.SportType
{
    public class SportTypeViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int SportCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class SportTypeTranslationViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int SportTypeId { get; set; }
        public int LanguageId { get; set; }
        public LanguageViewModel? Language { get; set; }
    }
}
