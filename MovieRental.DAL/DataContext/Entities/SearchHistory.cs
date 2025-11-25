namespace MovieRental.DAL.DataContext.Entities
{
    public class SearchHistory : TimeStample
    {
        public required string UserId { get; set; }
        public AppUser User { get; set; } = null!;

        public required string SearchQuery { get; set; }
        public string? SearchType { get; set; } // "Movie", "Event", "Sport", "Actor"
        public DateTime SearchDate { get; set; }  
    }
}
