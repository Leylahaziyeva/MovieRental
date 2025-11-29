namespace MovieRental.BLL.ViewModels.Event
{
    public class EventFilterResultViewModel
    {
        public List<EventViewModel> Events { get; set; } = new();
        public int TotalCount { get; set; }
        public EventFilterViewModel Filter { get; set; } = new();

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / Filter.PageSize);
        public bool HasPreviousPage => Filter.Page > 1;
        public bool HasNextPage => Filter.Page < TotalPages;
    }
}