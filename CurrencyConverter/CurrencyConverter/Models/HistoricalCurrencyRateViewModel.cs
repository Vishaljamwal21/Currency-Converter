namespace CurrencyConverter.Models
{
    public class HistoricalCurrencyRateViewModel
    {
        public DateTime Date { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? Rate { get; set; }
    }
}
