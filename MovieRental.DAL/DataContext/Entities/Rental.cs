namespace MovieRental.DAL.DataContext.Entities
{
    public class Rental : TimeStample
    {
        public required string UserId { get; set; }
        public AppUser? User { get; set; } 

        public int MovieId { get; set; }
        public Movie? Movie { get; set; } 

        public DateTime RentalDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;
        public bool HasWatched { get; set; }
    }
}