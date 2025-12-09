namespace MovieRental.DAL.DataContext.Entities
{
    public class EventCategory : TimeStample
    {
        public List<EventCategoryTranslation> Translations { get; set; } = [];
        public List<Event> Events { get; set; } = [];
    }

    public class EventCategoryTranslation : TimeStample
    {
        public required string Name { get; set; }
        public int EventCategoryId { get; set; }
        public EventCategory? EventCategory { get; set; }
        public int LanguageId { get; set; }
        public Language? Language { get; set; }
    }
}
