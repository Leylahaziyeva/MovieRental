namespace MovieRental.DAL.DataContext.Entities
{
    public class Rental : TimeStample
    {
        public required string UserId { get; set; }
        public virtual AppUser User { get; set; } = null!;

        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; } = null!;

        public DateTime RentalDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;
        public bool HasWatched { get; set; }
    }
}