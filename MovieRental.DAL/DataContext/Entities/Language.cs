namespace MovieRental.DAL.DataContext.Entities
{
    public class Language : TimeStample
    {
        public required string Name { get; set; }
        public required string IsoCode { get; set; }
        public required string ImageUrl { get; set; }
    }
}
