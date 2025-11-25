namespace MovieRental.DAL.DataContext.Entities
{
    public class Genre : TimeStample
    {
        public string? IconClass { get; set; }  // fa-film, fa-heart

        public List<GenreTranslation> GenreTranslations { get; set; } = new List<GenreTranslation>();
        public List<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
    }

    public class GenreTranslation : TimeStample
    {
        public required string Name { get; set; }           
        public string? Description { get; set; }

        public int GenreId { get; set; }
        public virtual Genre Genre { get; set; } = null!;

        public int LanguageId { get; set; }
        public virtual Language Language { get; set; } = null!;
    }
}