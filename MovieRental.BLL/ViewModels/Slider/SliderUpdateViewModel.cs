namespace MovieRental.BLL.ViewModels.Slider
{
    public class SliderUpdateViewModel
    {
        public int Id { get; set; }
        public string? ImageUrl { get; set; }
        public int? MovieId { get; set; }
        public string? ActionUrl { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }

        public List<SliderTranslationUpdateViewModel>? Translations { get; set; }
    }

    public class SliderTranslationUpdateViewModel
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Description { get; set; }
        public string? ButtonText { get; set; }
        public int LanguageId { get; set; }
    }
}