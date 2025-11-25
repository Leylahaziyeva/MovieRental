namespace MovieRental.DAL.DataContext.Entities
{
    public class Currency : TimeStample
    {
        public required string Name { get; set; }          // USD, AZN, EUR
        public required string IsoCode { get; set; }       // usd, azn, eur
        public required string Symbol { get; set; }        // $, ₼, €
        public required decimal ExchangeRate { get; set; } // 1.0, 1.7, 0.85
    }
}
