using MovieRental.BLL.ViewModels.Language;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.ViewModels.Person
{
    public class PersonViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Biography { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? CoverImageUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? FormattedDateOfBirth { get; set; }
        public string? Gender { get; set; }
        public int KnownCredits { get; set; }
        public string? PlaceOfBirth { get; set; }
        public PersonType PersonType { get; set; }
        public string? PersonTypeDisplay { get; set; }
        public string? KnownFor { get; set; }

        public string? FacebookUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? YoutubeUrl { get; set; }
    }

    public class PersonTranslationViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Biography { get; set; }
        public int PersonId { get; set; }
        public PersonViewModel? Person { get; set; }
        public int LanguageId { get; set; }
        public LanguageViewModel? Language { get; set; }
    }
}
