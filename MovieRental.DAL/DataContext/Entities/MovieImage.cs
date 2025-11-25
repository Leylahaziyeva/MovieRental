namespace MovieRental.DAL.DataContext.Entities
{
    public class MovieImage : TimeStample
    {
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; } = null!;

        public required string ImageUrl { get; set; }  
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
    }
}