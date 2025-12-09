namespace MovieRental.BLL.ViewModels.Currency
{
    public class CurrencyViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? IsoCode { get; set; }
        public string? Symbol { get; set; }
        public decimal ExchangeRate { get; set; }
    }

    public class CurrencyTranslationViewModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int TranslationLanguageId { get; set; }
    }
}