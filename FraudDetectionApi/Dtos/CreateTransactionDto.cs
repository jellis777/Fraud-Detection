

using System.ComponentModel.DataAnnotations;

namespace FraudDetectionApi.Dtos
{
    public class CreateTransactionDto
    {
        [Required]
        public string AccountId { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required]
        public string Country { get; set; } = string.Empty;

        [Required]
        public string AccountHomeCountry { get; set; } = string.Empty;

        [Required]
        public string Merchant { get; set; } = string.Empty;

        public DateTime OccurredAt { get; set; }
    }
}