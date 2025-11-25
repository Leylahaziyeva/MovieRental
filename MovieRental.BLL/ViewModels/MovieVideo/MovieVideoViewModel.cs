namespace MovieRental.BLL.ViewModels.MovieVideo
{
    public class MovieVideoViewModel
    {
        public string VideoUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? VideoType { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}
