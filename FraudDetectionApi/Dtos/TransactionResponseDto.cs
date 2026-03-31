
using FraudDetectionApi.Enums;

namespace FraudDetectionApi.Dtos
{
    public class TransactionResponseDto
    {
        public int Id { get; set; }
        public string AccountId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Country { get; set; } = string.Empty;
        public string Merchant { get; set; } = string.Empty;
        public DateTime OccurredAt { get; set; }
        public FraudDecision Decision { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}