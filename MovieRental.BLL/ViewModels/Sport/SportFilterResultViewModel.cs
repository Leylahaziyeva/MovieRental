namespace MovieRental.BLL.ViewModels.Sport
{
    public class SportFilterResultViewModel
    {
        public List<SportViewModel> Sports { get; set; } = new();
        public int TotalCount { get; set; }
        public SportFilterViewModel Filter { get; set; } = new();
    }
}