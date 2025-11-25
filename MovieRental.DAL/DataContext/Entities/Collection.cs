namespace MovieRental.DAL.DataContext.Entities
{
    public class Collection : TimeStample
    {
        public required string ImageUrl { get; set; }            
        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public List<CollectionTranslation> CollectionTranslations { get; set; } = new List<CollectionTranslation>();
        public List<CollectionMovie> CollectionMovies { get; set; } = new List<CollectionMovie>();
    }

    public class CollectionTranslation : TimeStample
    {
        public required string Name { get; set; }            
        public string? Description { get; set; }

        public int CollectionId { get; set; }
        public virtual Collection Collection { get; set; } = null!;

        public int LanguageId { get; set; }
        public virtual Language Language { get; set; } = null!;
    }
}