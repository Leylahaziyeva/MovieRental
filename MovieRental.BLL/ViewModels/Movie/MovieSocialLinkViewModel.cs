namespace MovieRental.BLL.ViewModels.Movie
{
    public class MovieSocialLinkViewModel
    {
        public int Id { get; set; }
        public required string Platform { get; set; }
        public required string Url { get; set; }
        public string? IconClass { get; set; }
    }
}