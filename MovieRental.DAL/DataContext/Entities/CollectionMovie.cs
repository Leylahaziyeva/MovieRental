namespace MovieRental.DAL.DataContext.Entities
{
    public class CollectionMovie : TimeStample
    {
        public int CollectionId { get; set; }
        public virtual Collection Collection { get; set; } = null!;

        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; } = null!;

        public int DisplayOrder { get; set; }
    }
}