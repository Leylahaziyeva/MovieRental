namespace MovieRental.BLL.ViewModels.Language
{
    public class LanguageCreateViewModel
    {
        public required string Name { get; set; }
        public required string IsoCode { get; set; }
        public required string ImageUrl { get; set; }
        public List<LanguageTranslationCreateViewModel> Translations { get; set; } = [];
    }
    public class LanguageTranslationCreateViewModel
    {
        public required string Name { get; set; }
        public int TranslationLanguageId { get; set; }
    }

}
