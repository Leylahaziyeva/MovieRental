namespace MovieRental.BLL.ViewModels.Movie
{
    public class MovieSocialLinkCreateDto
    {
        public required string Platform { get; set; }
        public required string Url { get; set; }
        public string? IconClass { get; set; }
        public int DisplayOrder { get; set; }
    }
}