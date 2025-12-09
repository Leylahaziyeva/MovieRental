using MovieRental.BLL.ViewModels.Language;

namespace MovieRental.BLL.ViewModels.Location
{
    public class LocationViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int SportCount { get; set; }
        public int EventCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class LocationTranslationViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int LocationId { get; set; }
        public int LanguageId { get; set; }
        public LanguageViewModel? Language { get; set; }
    }
}