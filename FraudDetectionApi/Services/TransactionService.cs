using FraudDetectionApi.Data;
using FraudDetectionApi.Dtos;
using FraudDetectionApi.Enums;
using FraudDetectionApi.Interfaces;
using FraudDetectionApi.Models;
using Microsoft.EntityFrameworkCore;


namespace FraudDetectionApi.Services
{
    public class TransactionService : ITransactionService
    {

        private readonly AppDbContext _context;
        private readonly IFraudRuleEngine _fraudRuleEngine;
        private readonly ILogger<TransactionService> _logger;
        private static readonly HashSet<string> BlockedCountries = new(StringComparer.OrdinalIgnoreCase)
        {
            "North Korea",
            "Iran"
        };

        public TransactionService(AppDbContext context, IFraudRuleEngine fraudRuleEngine, ILogger<TransactionService> logger)
        {
            _context = context;
            _fraudRuleEngine = fraudRuleEngine;
            _logger = logger;
        }
        public async Task<TransactionResponseDto> CreateTransactionAsync(CreateTransactionDto dto)
        {
            _logger.LogInformation(
                "Received transaction for account {AccountId}, amount {Amount}, country {Country}, merchant {Merchant}",
                dto.AccountId,
                dto.Amount,
                dto.Country,
                dto.Merchant);


            var oneMinuteAgo = dto.OccurredAt.AddMinutes(-1);

            var recentCount = await _context.Transactions.CountAsync(t =>
            t.AccountId == dto.AccountId &&
            t.OccurredAt >= oneMinuteAgo &&
            t.OccurredAt <= dto.OccurredAt);

            _logger.LogInformation(
                "Found {RecentCount} recent transactions for account {AccountId} in the last minute",
                recentCount,
                dto.AccountId
            );

            var evaluation = _fraudRuleEngine.Evaluate(dto, recentCount);

            if (evaluation.Decision == FraudDecision.Suspicious)
            {
                _logger.LogWarning(
                    "Transaction for account {AccountId} marked suspicious. Reasons: {Reasons}",
                    dto.AccountId,
                    string.Join(" | ", evaluation.Reasons)
                );
            }
            else if (evaluation.Decision == FraudDecision.Fraud)
            {
                _logger.LogWarning(
                    "Transaction for account {AccountId} marked as fraud. Reasons: {Reasons}",
                    dto.AccountId,
                    string.Join(" | ", evaluation.Reasons)
                );
            }

            _logger.LogInformation(
                "Fraud evaluation completed for account {AccountId}. Decision: {Decision}. Reasons: {Reasons}",
                dto.AccountId,
                evaluation.Decision,
                string.Join(" | ", evaluation.Reasons)
            );

            var transaction = new Transaction
            {
                AccountId = dto.AccountId,
                Amount = dto.Amount,
                Country = dto.Country,
                Merchant = dto.Merchant,
                OccurredAt = dto.OccurredAt,
                Decision = evaluation.Decision,
                Reason = string.Join(" ", evaluation.Reasons),
                CreatedAt = DateTime.UtcNow
            };

            _logger.LogInformation(
                "Transaction {TransactionId} saved successfully for account {AccountId} with decision {Decision}",
                transaction.Id,
                transaction.AccountId,
                transaction.Decision
            );

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return MapToDto(transaction);
        }

        public async Task<List<TransactionResponseDto>> GetAllTransactionsAsync()
        {
            var transactions = await _context.Transactions
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();


            return transactions.Select(MapToDto).ToList();
        }

        public async Task<TransactionResponseDto?> GetTransactionByIdAsync(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction == null) return null;

            return MapToDto(transaction);
        }

        public async Task<bool> DeleteTransactionAsync(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);

            if (transaction is null)
            {
                return false;
            }

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Transaction {TransactionId} deleted successfully for account {AccountId}",
                transaction.Id,
                transaction.AccountId
            );

            return true;
        }

        private static TransactionResponseDto MapToDto(Transaction t)
        {
            return new TransactionResponseDto
            {
                Id = t.Id,
                AccountId = t.AccountId,
                Amount = t.Amount,
                Country = t.Country,
                Merchant = t.Merchant,
                OccurredAt = t.OccurredAt,
                Decision = t.Decision,
                Reason = t.Reason
            };
        }

    }
}