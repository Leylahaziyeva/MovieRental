namespace MovieRental.BLL.ViewModels.MovieImage
{
    public class MovieImageViewModel
    {
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
    }
}
