namespace MovieRental.DAL.DataContext.Entities
{
    public class Language : TimeStample
    {
        public required string IsoCode { get; set; }
        public required string ImageUrl { get; set; }
        public List<LanguageTranslation> LanguageTranslations { get; set; } = [];
    }

    public class LanguageTranslation : TimeStample
    {
        public required string Name { get; set; }
        public int LanguageId { get; set; }   
        public Language? Language { get; set; }

        public int TranslationLanguageId { get; set; } 
        public Language? TranslationLanguage { get; set; }
    }
}
