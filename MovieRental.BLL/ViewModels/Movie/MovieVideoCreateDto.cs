using Microsoft.AspNetCore.Http;

namespace MovieRental.BLL.ViewModels.Movie
{
    public class MovieVideoCreateDto
    {
        public required IFormFile VideoFile { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public int VideoType { get; set; }  
    }
}