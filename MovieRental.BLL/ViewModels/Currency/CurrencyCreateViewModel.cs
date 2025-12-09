namespace MovieRental.BLL.ViewModels.Currency
{
    public class CurrencyCreateViewModel
    {
        public required string Name { get; set; }
        public required string IsoCode { get; set; }
        public required string Symbol { get; set; }
        public decimal ExchangeRate { get; set; }
        public List<CurrencyTranslationCreateViewModel> Translations { get; set; } = new();
    }
    public class CurrencyTranslationCreateViewModel
    {
        public required string Name { get; set; }
        public int TranslationLanguageId { get; set; }
    }
}