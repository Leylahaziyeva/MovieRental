namespace MovieRental.BLL.ViewModels.Movie
{
    public class MovieImageViewModel
    {
        public int Id { get; set; }
        public required string ImageUrl { get; set; }
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }
    }
}