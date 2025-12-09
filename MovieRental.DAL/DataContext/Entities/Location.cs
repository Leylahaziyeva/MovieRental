namespace MovieRental.DAL.DataContext.Entities
{
    public class Location : TimeStample
    {
        public List<LocationTranslation> Translations { get; set; } = [];
        public List<Sport> Sports { get; set; } = [];
        public List<Event> Events { get; set; } = [];
    }

    public class LocationTranslation : TimeStample
    {
        public required string Name { get; set; }
        public int LocationId { get; set; }
        public Location? Location { get; set; }
        public int LanguageId { get; set; }
        public Language? Language { get; set; }
    }
}