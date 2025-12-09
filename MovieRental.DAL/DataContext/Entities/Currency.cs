namespace MovieRental.DAL.DataContext.Entities
{
    public class Currency : TimeStample
    {     
        public required string IsoCode { get; set; }      
        public required string Symbol { get; set; }       
        public required decimal ExchangeRate { get; set; }

        public List<CurrencyTranslation> Translations { get; set; } = [];
    }

    public class CurrencyTranslation : TimeStample
    {
        public required string Name { get; set; }
        public int CurrencyId { get; set; }
        public Currency? Currency { get; set; } 
        public int LanguageId { get; set; }
        public Language? Language { get; set; }

    }
}
