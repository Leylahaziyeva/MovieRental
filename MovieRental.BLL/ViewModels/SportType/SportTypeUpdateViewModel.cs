using MovieRental.BLL.ViewModels.Language;

namespace MovieRental.BLL.ViewModels.SportType
{
    public class SportTypeUpdateViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }
    public class SportTypeTranslationUpdateViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int SportTypeId { get; set; }
        public int LanguageId { get; set; }
        public LanguageViewModel? Language { get; set; }
    }
}