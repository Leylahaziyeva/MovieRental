namespace MovieRental.BLL.ViewModels.Actor
{
    public class ActorViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public string? Role { get; set; }
        public string Category { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
