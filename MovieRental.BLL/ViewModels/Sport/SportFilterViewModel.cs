using MovieRental.BLL.ViewModels.Language;
using MovieRental.BLL.ViewModels.Location;
using MovieRental.BLL.ViewModels.SportType;

namespace MovieRental.BLL.ViewModels.Sport
{
    public class SportFilterViewModel
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<SportTypeOption> SportTypes { get; set; } = new();
        public List<LocationOption> Locations { get; set; } = new();
        public List<LanguageViewModel> Languages { get; set; } = new();
    }
}