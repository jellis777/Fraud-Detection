using FraudDetectionApi.Data;
using FraudDetectionApi.Dtos;
using FraudDetectionApi.Enums;
using FraudDetectionApi.Interfaces;
using FraudDetectionApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace FraudDetectionApi.Tests
{
    public class TransactionServiceTests
    {
        private static AppDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        private readonly FraudRuleEngine _engine = new();
        private readonly ILogger<TransactionService> _logger = NullLogger<TransactionService>.Instance;


        [Fact]
        public async Task CreateTransactionAsync_Returns_Approved_For_Normal_Transaction()
        {
            using var context = CreateDbContext();
            var service = new TransactionService(context, _engine, _logger);

            var dto = new CreateTransactionDto
            {
                AccountId = "acct-1",
                Amount = 100,
                Country = "United States",
                AccountHomeCountry = "United States",
                Merchant = "Coffee Shop",
                OccurredAt = DateTime.UtcNow

            };

            var result = await service.CreateTransactionAsync(dto);

            Assert.Equal(FraudDecision.Approved, result.Decision);
        }

        [Fact]
        public async Task CreateTransactionAsync_Returns_Suspicious_For_High_Amount()
        {
            using var context = CreateDbContext();
            var service = new TransactionService(context, _engine, _logger);

            var dto = new CreateTransactionDto
            {
                AccountId = "acct-2",
                Amount = 7000,
                Country = "United States",
                AccountHomeCountry = "United States",
                Merchant = "Electronics Store",
                OccurredAt = DateTime.UtcNow
            };

            var result = await service.CreateTransactionAsync(dto);

            Assert.Equal(FraudDecision.Suspicious, result.Decision);
        }

        [Fact]
        public async Task CreateTransactionAsync_Returns_Fraud_For_Blocked_Country()
        {
            using var context = CreateDbContext();
            var service = new TransactionService(context, _engine, _logger);

            var dto = new CreateTransactionDto
            {
                AccountId = "acct-2",
                Amount = 7000,
                Country = "Iran",
                AccountHomeCountry = "United States",
                Merchant = "Electronics Store",
                OccurredAt = DateTime.UtcNow
            };

            var result = await service.CreateTransactionAsync(dto);

            Assert.Equal(FraudDecision.Fraud, result.Decision);

        }

        [Fact]
        public async Task CreateTransactionAsync_Returns_Fraud_For_Too_Many_Recent_Transactions()
        {
            using var context = CreateDbContext();
            var service = new TransactionService(context, _engine, _logger);

            var baseTime = DateTime.UtcNow;

            for (int i = 0; i < 3; i++)
            {
                await service.CreateTransactionAsync(new CreateTransactionDto
                {
                    AccountId = "acct-4",
                    Amount = 100,
                    Country = "United States",
                    AccountHomeCountry = "United States",
                    Merchant = "Electronics Store",
                    OccurredAt = DateTime.UtcNow
                });
            }
            var fourth = await service.CreateTransactionAsync(new CreateTransactionDto
            {
                AccountId = "acct-4",
                Amount = 100,
                Country = "United States",
                AccountHomeCountry = "United States",
                Merchant = "Electronics Store",
                OccurredAt = DateTime.UtcNow
            });



            Assert.Equal(FraudDecision.Fraud, fourth.Decision);

        }
    }
}