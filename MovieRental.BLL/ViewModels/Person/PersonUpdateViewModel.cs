using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieRental.DAL.DataContext.Entities;

namespace MovieRental.BLL.ViewModels.Person
{
    public class PersonUpdateViewModel
    {
        public int Id { get; set; }
        public IFormFile? ProfileImageFile { get; set; }
        public string? ProfileImageUrl { get; set; }
        public IFormFile? CoverImageFile { get; set; }
        public string? CoverImageUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; }
        public int KnownCredits { get; set; }
        public string? PlaceOfBirth { get; set; }
        public PersonType PersonType { get; set; }
        public List<SelectListItem>? PersonTypeList { get; set; }
        public string? KnownFor { get; set; }

        public string? FacebookUrl { get; set; }
        public string? TwitterUrl { get; set; }
        public string? InstagramUrl { get; set; }
        public string? YoutubeUrl { get; set; }
    }

    public class PersonTranslationUpdateViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Biography { get; set; }
        public int PersonId { get; set; }
        public int LanguageId { get; set; }
        public List<SelectListItem>? Languages { get; set; }
    }
}
