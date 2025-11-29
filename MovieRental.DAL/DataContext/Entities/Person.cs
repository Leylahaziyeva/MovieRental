namespace MovieRental.DAL.DataContext.Entities
{
    public class Person : TimeStample
    {
        public required string ProfileImageUrl { get; set; }
        public required string CoverImageUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public int KnownCredits { get; set; }
        public string? PlaceOfBirth { get; set; }
        public PersonType PersonType { get; set; } // Actor, Artist, Sportsman
        public string? KnownFor { get; set; } // Acting, Music, Football etc.

        public string? FacebookUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? YoutubeUrl { get; set; }

        public List<PersonTranslation>? PersonTranslations { get; set; }
    }

    public class PersonTranslation : TimeStample
    {
        public required string Name { get; set; }
        public required string Biography { get; set; }
        public int PersonId { get; set; }
        public Person? Person { get; set; }
        public int LanguageId { get; set; }
        public Language? Language { get; set; }
    }

    public enum PersonType
    {
        Actor = 1,
        Artist = 2,
        Sportsman = 3
    }
}
