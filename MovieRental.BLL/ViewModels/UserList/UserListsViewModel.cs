namespace MovieRental.BLL.ViewModels.UserList
{
    public class UserListsViewModel
    {
        public IEnumerable<UserListViewModel> Lists { get; set; } = [];
        public int TotalCount { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;

        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool HasPreviousPage => Page > 1;
        public bool HasNextPage => Page < TotalPages;
    }
}