namespace MovieRental.DAL.DataContext.Entities
{
    public class SportType : TimeStample
    {
        public List<SportTypeTranslation> Translations { get; set; } = [];
        public List<Sport> Sports { get; set; } = [];
    }

    public class SportTypeTranslation : TimeStample
    {
        public required string Name { get; set; }
        public int SportTypeId { get; set; }
        public SportType? SportType { get; set; }
        public int LanguageId { get; set; }
        public Language? Language { get; set; }
    }
}