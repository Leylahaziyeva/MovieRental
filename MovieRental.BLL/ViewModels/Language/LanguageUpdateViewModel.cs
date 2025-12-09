namespace MovieRental.BLL.ViewModels.Language
{
    public class LanguageUpdateViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string IsoCode { get; set; }
        public required string ImageUrl { get; set; }
        public List<LanguageTranslationUpdateViewModel> Translations { get; set; } = [];
    }
    public class LanguageTranslationUpdateViewModel
    {
        public int? Id { get; set; }
        public required string Name { get; set; }
        public int TranslationLanguageId { get; set; }
    }

}
