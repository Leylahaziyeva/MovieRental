namespace MovieRental.DAL.DataContext.Entities
{
    public class MovieActor : TimeStample
    {
        public int MovieId { get; set; }
        public virtual Movie Movie { get; set; } = null!;

        public int ActorId { get; set; }
        public virtual Actor Actor { get; set; } = null!;

        public string? Role { get; set; }              // Character name or "Director", "Writer", "Producer"
        public required string Category { get; set; }  // "Cast", "Crew", "Support"
        public string? JobTitle { get; set; }         // "Acting", "Director", "Cinematographer", "Producer"
        public int DisplayOrder { get; set; }
    }
}