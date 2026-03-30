

namespace FraudDetectionApi.Dtos
{
    public class CreateTransactionDto
    {
        public string AccountId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Country { get; set; } = string.Empty;
        public string AccountHomeCountry { get; set; } = string.Empty;
        public string Merchant { get; set; } = string.Empty;
        public DateTime OccuredAt { get; set; }
    }
}