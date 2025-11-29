namespace MovieRental.DAL.DataContext.Entities
{
    public class ReviewReaction : TimeStample
    {
        public int ReviewId { get; set; }
        public Review? Review { get; set; }

        public required string UserId { get; set; }
        public AppUser? User { get; set; } 

        public required string ReactionType { get; set; }  // "Helpful", "Not Helpful", "Like"
        public DateTime ReactionDate { get; set; }
    }
}
