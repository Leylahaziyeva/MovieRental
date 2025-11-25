namespace MovieRental.DAL.DataContext.Entities
{
    public class MovieTab : TimeStample
    {
        public required string TabKey { get; set; }    // "summary", "cast", "videos", "reviews"

        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual List<MovieTabTranslation> MovieTabTranslations { get; set; } = new List<MovieTabTranslation>();
    }

    public class MovieTabTranslation : TimeStample
    {
        public required string Name { get; set; }      // "Summary", "Cast", "Videos", "Reviews"

        public int MovieTabId { get; set; }
        public virtual MovieTab MovieTab { get; set; } = null!;

        public int LanguageId { get; set; }
        public virtual Language Language { get; set; } = null!;
    }
}
