namespace MovieRental.DAL.DataContext.Entities
{
    public class SearchHistory : TimeStample
    {
        public required string UserId { get; set; }
        public AppUser? User { get; set; } 

        public required string SearchQuery { get; set; }
        public string? SearchType { get; set; } 
        public DateTime SearchDate { get; set; }  
    }
}
