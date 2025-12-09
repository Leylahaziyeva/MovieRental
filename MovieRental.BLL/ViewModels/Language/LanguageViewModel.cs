namespace MovieRental.BLL.ViewModels.Language
{
    public class LanguageViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? IsoCode { get; set; }
        public string? ImageUrl { get; set; }
    }
    public class LanguageTranslationViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int TranslationLanguageId { get; set; }
    }

}
