namespace MovieRental.BLL.ViewModels.Event
{
    public class EventFilterResultViewModel
    {
        public List<EventViewModel> Events { get; set; } = new();
        public int TotalCount { get; set; }
        public EventFilterViewModel Filter { get; set; } = new();
    }
}