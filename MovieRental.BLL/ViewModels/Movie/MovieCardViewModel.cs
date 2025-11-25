namespace MovieRental.BLL.ViewModels.Movie
{
    public class MovieCardViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string PosterImageUrl { get; set; } = string.Empty;
        public int Year { get; set; }
        public int Duration { get; set; }
        public decimal RentalPrice { get; set; }
        public int LovePercentage { get; set; }
        public int VotesCount { get; set; }
        public string Format { get; set; } = string.Empty;
        public List<string> Genres { get; set; } = [];
        public bool IsFeatured { get; set; }
        public bool IsAvailableForRent { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string Language { get; set; } = string.Empty;

        //Computed properties 
        public int Rating => LovePercentage;
        public string PosterUrl => PosterImageUrl;
    }
}
