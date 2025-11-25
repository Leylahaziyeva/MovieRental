namespace MovieRental.BLL.ViewModels
{
    public class SliderViewModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public int? MovieId { get; set; }
        public string? ActionUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }

        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Description { get; set; }
        public string? ButtonText { get; set; }
    }


    public class SliderTranslationViewModel
    {
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Description { get; set; }
        public string? ButtonText { get; set; }

        public int LanguageId { get; set; }
    }
}
