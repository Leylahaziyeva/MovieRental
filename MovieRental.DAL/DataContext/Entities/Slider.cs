namespace MovieRental.DAL.DataContext.Entities
{
    public class Slider : TimeStample
    {
        public required string ImageUrl { get; set; }     

        public int? MovieId { get; set; }
        public Movie? Movie { get; set; }

        public string? ActionUrl { get; set; }

        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual ICollection<SliderTranslation> SliderTranslations { get; set; } = [];
    }

    public class SliderTranslation : TimeStample
    {
        public string? Title { get; set; }                   // "ABOMINABLE"
        public string? Subtitle { get; set; }                // "HOME HAS A MAGIC ALL ITS OWN"
        public string? Description { get; set; }             // Optional
        public string? ButtonText { get; set; }              // "Book Now"

        public int SliderId { get; set; }
        public  Slider? Slider { get; set; }

        public int LanguageId { get; set; }
        public Language? Language { get; set; } 
    }
}