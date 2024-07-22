namespace CurrencyConverter.Models
{
    public class FavoriteCurrencyPair
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
    }
}
