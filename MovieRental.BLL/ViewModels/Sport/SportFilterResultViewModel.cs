namespace MovieRental.BLL.ViewModels.Sport
{
    namespace MovieRental.BLL.ViewModels.Sport
    {
        public class SportFilterResultViewModel
        {
            public IEnumerable<SportViewModel> Sports { get; set; } = [];
            public SportFilterViewModel Filter { get; set; } = new();
            public int TotalPages { get; set; }
            public bool HasNextPage { get; set; }
            public bool HasPreviousPage { get; set; }
        }
    }
}
