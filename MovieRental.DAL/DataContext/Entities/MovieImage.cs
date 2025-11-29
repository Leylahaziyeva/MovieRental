namespace MovieRental.DAL.DataContext.Entities
{
    public class MovieImage : TimeStample
    {
        public int MovieId { get; set; }
        public Movie? Movie { get; set; }

        public required string ImageUrl { get; set; }  
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}