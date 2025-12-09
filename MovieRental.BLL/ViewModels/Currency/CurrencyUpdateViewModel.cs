namespace MovieRental.BLL.ViewModels.Currency
{
    public class CurrencyUpdateViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string IsoCode { get; set; }
        public required string Symbol { get; set; }
        public decimal ExchangeRate { get; set; }
        public List<CurrencyTranslationUpdateViewModel> Translations { get; set; } = new();
    }
    public class CurrencyTranslationUpdateViewModel
    {
        public int? Id { get; set; }
        public required string Name { get; set; }
        public int TranslationLanguageId { get; set; }
    }
}