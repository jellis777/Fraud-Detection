using FraudDetectionApi.Dtos;
using FraudDetectionApi.Models;

namespace FraudDetectionApi.Interfaces
{
    public interface IFraudRuleEngine
    {
        FraudEvaluationResult Evaluate(CreateTransactionDto dto, int recentTransactionCount);
    }
}