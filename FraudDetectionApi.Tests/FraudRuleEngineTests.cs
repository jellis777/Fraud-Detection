using FraudDetectionApi.Dtos;
using FraudDetectionApi.Enums;
using FraudDetectionApi.Services;

namespace FraudDetectionApi.Tests
{
    public class FraudRuleEngineTests
    {
        private readonly FraudRuleEngine _engine = new();

        [Fact]
        public void Evaluate_Returns_Approved_For_Normal_Transaction()
        {
            var dto = new CreateTransactionDto
            {
                AccountId = "acct-1",
                Amount = 100,
                Country = "United States",
                AccountHomeCountry = "United States",
                Merchant = "Coffee Shop",
                OccurredAt = DateTime.UtcNow
            };

            var result = _engine.Evaluate(dto, recentTransactionCount: 0);

            Assert.Equal(FraudDecision.Approved, result.Decision);
            Assert.Contains("Transaction approved.", result.Reasons);
        }

        [Fact]
        public void Evaluate_Returns_Suspicious_For_High_Amount()
        {
            var dto = new CreateTransactionDto
            {
                AccountId = "acct-2",
                Amount = 7000,
                Country = "United States",
                AccountHomeCountry = "United States",
                Merchant = "Electronics Store",
                OccurredAt = DateTime.UtcNow
            };

            var result = _engine.Evaluate(dto, recentTransactionCount: 0);

            Assert.Equal(FraudDecision.Suspicious, result.Decision);
            Assert.Contains("Amount exceeds suspicious threshold.", result.Reasons);
        }

        [Fact]
        public void Evaluate_Returns_Suspicious_For_Foreign_Country()
        {
            var dto = new CreateTransactionDto
            {
                AccountId = "acct-3",
                Amount = 200,
                Country = "Canada",
                AccountHomeCountry = "United States",
                Merchant = "Store",
                OccurredAt = DateTime.UtcNow
            };

            var result = _engine.Evaluate(dto, recentTransactionCount: 0);

            Assert.Equal(FraudDecision.Suspicious, result.Decision);
            Assert.Contains("Transaction originated outside account home country.", result.Reasons);
        }

        [Fact]
        public void Evaluate_Returns_Fraud_For_Blocked_Country()
        {
            var dto = new CreateTransactionDto
            {
                AccountId = "acct-4",
                Amount = 200,
                Country = "Iran",
                AccountHomeCountry = "United States",
                Merchant = "Wire Service",
                OccurredAt = DateTime.UtcNow
            };

            var result = _engine.Evaluate(dto, recentTransactionCount: 0);

            Assert.Equal(FraudDecision.Fraud, result.Decision);
            Assert.Contains("Transaction originated from blocked country.", result.Reasons);
        }

        [Fact]
        public void Evaluate_Returns_Fraud_For_Very_High_Amount()
        {
            var dto = new CreateTransactionDto
            {
                AccountId = "acct-5",
                Amount = 12000,
                Country = "United States",
                AccountHomeCountry = "United States",
                Merchant = "Luxury Store",
                OccurredAt = DateTime.UtcNow
            };

            var result = _engine.Evaluate(dto, recentTransactionCount: 0);

            Assert.Equal(FraudDecision.Fraud, result.Decision);
            Assert.Contains("Amount exceeds fraud threshold.", result.Reasons);
        }

        [Fact]
        public void Evaluate_Returns_Fraud_For_Too_Many_Recent_Transactions()
        {
            var dto = new CreateTransactionDto
            {
                AccountId = "acct-6",
                Amount = 100,
                Country = "United States",
                AccountHomeCountry = "United States",
                Merchant = "Store",
                OccurredAt = DateTime.UtcNow
            };

            var result = _engine.Evaluate(dto, recentTransactionCount: 3);

            Assert.Equal(FraudDecision.Fraud, result.Decision);
            Assert.Contains("Too many recent transactions for this account.", result.Reasons);
        }
    }
}
