namespace MovieRental.DAL.DataContext.Entities
{
    public class Review : TimeStample
    {
        public int MovieId { get; set; }
        public Movie? Movie { get; set; } 

        public required string UserId { get; set; }
        public AppUser? User { get; set; } 

        public int Rating { get; set; }  // 1-5 ulduz
        public required string Comment { get; set; }
        public DateTime ReviewDate { get; set; }

        public bool IsApproved { get; set; }

        public int? ParentReviewId { get; set; }
        public Review? ParentReview { get; set; }
        public List<Review> Replies { get; set; } = [];

        public List<ReviewReaction> ReviewReactions { get; set; } = [];
    }
}
