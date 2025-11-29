namespace MovieRental.BLL.ViewModels.Movie
{
    public class MovieVideoViewModel
    {
        public int Id { get; set; }
        public required string VideoUrl { get; set; }
        public string? Title { get; set; }  
        public string? Description { get; set; } 
        public string? VideoType { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}