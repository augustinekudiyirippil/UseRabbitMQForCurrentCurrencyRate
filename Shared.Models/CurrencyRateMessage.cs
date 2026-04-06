namespace Shared.Models
{
    public class CurrencyRateMessage
    {
        public string Base { get; set; }
        public string Currency { get; set; }
        public decimal Rate { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
