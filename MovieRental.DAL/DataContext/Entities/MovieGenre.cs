namespace MovieRental.DAL.DataContext.Entities
{
    public class MovieGenre : TimeStample
    {
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; } = null!;

        public int GenreId { get; set; }
        public virtual Genre Genre { get; set; } = null!;
    }
}