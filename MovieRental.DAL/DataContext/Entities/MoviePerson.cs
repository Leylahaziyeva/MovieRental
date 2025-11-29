namespace MovieRental.DAL.DataContext.Entities
{
    public class MoviePerson : TimeStample
    {
        public int MovieId { get; set; }
        public Movie? Movie { get; set; }

        public int PersonId { get; set; }
        public Person? Person { get; set; }

        public MoviePersonRole Role { get; set; }     
        public string? CharacterName { get; set; }     
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public enum MoviePersonRole
    {
        Actor = 1,
        Director = 2,
        Writer = 3,
        Producer = 4,
        Cinematographer = 5,
        Composer = 6
    }
}