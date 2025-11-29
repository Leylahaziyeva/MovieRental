using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.ViewModels.Person
{
    public class PersonListViewModel
    {
        public List<PersonViewModel>? People { get; set; }
        public PersonType? FilterByType { get; set; }
        public string? SearchQuery { get; set; }
        public string? SortBy { get; set; } // List Order, Date Added
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
    }
}
