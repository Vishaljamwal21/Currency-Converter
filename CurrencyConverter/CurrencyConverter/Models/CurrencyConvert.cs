namespace CurrencyConverter.Models
{
    public class CurrencyConvert
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string BaseCurrency { get; set; }
        public string TargetCurrency { get; set; }
        public decimal Amount { get; set; }
        public decimal ConvertedAmount { get; set; }
        public DateTime ConversionDate { get; set; }
    }
}
