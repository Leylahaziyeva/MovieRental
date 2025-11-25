namespace MovieRental.DAL.DataContext.Entities
{
    public class Actor : TimeStample
    {
        public required string ProfileImageUrl { get; set; }     
        public DateTime? BirthDate { get; set; }
        public string? Nationality { get; set; }

        public virtual ICollection<ActorTranslation> ActorTranslations { get; set; } = new List<ActorTranslation>();
        public virtual ICollection<MovieActor> MovieActors { get; set; } = new List<MovieActor>();
    }

    public class ActorTranslation : TimeStample
    {
        public required string FullName { get; set; }          
        public string? Biography { get; set; }                

        public int ActorId { get; set; }
        public virtual Actor Actor { get; set; } = null!;

        public int LanguageId { get; set; }
        public virtual Language Language { get; set; } = null!;
    }
}