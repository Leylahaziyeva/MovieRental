namespace MovieRental.DAL.DataContext.Entities
{
    public class Genre : TimeStample
    {
        public List<GenreTranslation> GenreTranslations { get; set; } = [];
        public List<MovieGenre> MovieGenres { get; set; } = [];
    }

    public class GenreTranslation : TimeStample
    {
        public required string Name { get; set; }  
        
        public int GenreId { get; set; }
        public Genre? Genre { get; set; }

        public int LanguageId { get; set; }
        public Language? Language { get; set; }
    }
}