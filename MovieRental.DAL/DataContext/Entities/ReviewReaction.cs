namespace MovieRental.DAL.DataContext.Entities
{
    public class ReviewReaction : TimeStample
    {
        public int ReviewId { get; set; }
        public virtual Review Review { get; set; } = null!;

        public required string UserId { get; set; }
        public virtual AppUser User { get; set; } = null!;

        public required string ReactionType { get; set; }  // "Helpful", "Not Helpful", "Like"
        public DateTime ReactionDate { get; set; }
    }
}
