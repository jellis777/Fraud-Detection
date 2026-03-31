using FraudDetectionApi.Dtos;
using FraudDetectionApi.Enums;
using FraudDetectionApi.Interfaces;
using FraudDetectionApi.Models;

namespace FraudDetectionApi.Services
{
    public class FraudRuleEngine : IFraudRuleEngine
    {
        private static readonly HashSet<string> BlockedCountries = new(StringComparer.OrdinalIgnoreCase)
        {
            "North Korea",
            "Iran"
        };
        public FraudEvaluationResult Evaluate(CreateTransactionDto dto, int recentTransactionCount)
        {
            var result = new FraudEvaluationResult
            {
                Decision = FraudDecision.Approved

            };

            if (dto.Amount > 5000)
            {
                result.Decision = FraudDecision.Suspicious;
                result.Reasons.Add("Amount exceeds suspicious threshold.");
            }

            if (!string.Equals(dto.Country, dto.AccountHomeCountry, StringComparison.OrdinalIgnoreCase))
            {
                if (result.Decision != FraudDecision.Fraud)
                {
                    result.Decision = FraudDecision.Suspicious;
                }
                result.Reasons.Add("Transaction originated outside account home country.");
            }

            if (dto.Amount > 10000)
            {
                result.Decision = FraudDecision.Fraud;
                result.Reasons.Add("Amount exceeds fraud threshold.");
            }

            if (BlockedCountries.Contains(dto.Country))
            {
                result.Decision = FraudDecision.Fraud;
                result.Reasons.Add("Transaction originated from blocked country.");
            }

            if (recentTransactionCount >= 3)
            {
                result.Decision = FraudDecision.Fraud;
                result.Reasons.Add("Too many recent transactions for this account.");
            }

            if (result.Reasons.Count == 0)
            {
                result.Reasons.Add("Transaction approved.");
            }

            return result;

        }
    }
}
