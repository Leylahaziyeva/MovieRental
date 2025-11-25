namespace MovieRental.BLL.ViewModels.Sport
{
    public class SportCreateViewModel
    {
        public required string ImageUrl { get; set; }
        public DateTime EventDate { get; set; }
        public required string Location { get; set; }
        public decimal? Price { get; set; }
        public int? CurrencyId { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsFeatured { get; set; }

        public List<SportTranslationViewModel> Translations { get; set; } = new();
    }
}