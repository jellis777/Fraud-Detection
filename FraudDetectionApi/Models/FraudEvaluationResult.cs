using FraudDetectionApi.Enums;

namespace FraudDetectionApi.Models
{
    public class FraudEvaluationResult
    {
        public FraudDecision Decision { get; set; }
        public List<string> Reasons { get; set; } = new();
    }
}