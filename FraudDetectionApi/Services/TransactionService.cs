

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
        private static readonly HashSet<string> BlockedCountries = new(StringComparer.OrdinalIgnoreCase)
        {
            "North Korea",
            "Iran"
        };

        public TransactionService(AppDbContext context)
        {
            _context = context;
        }
        public async Task<Transaction> CreateTransactionAsync(CreateTransactionDto dto)
        {
            var decision = FraudDecision.Approved;
            var reasons = new List<string>();

            if (dto.Amount > 5000)
            {
                decision = FraudDecision.Suspicious;
                reasons.Add("Amount exceeds suspicious threshold.");
            }

            if (!string.Equals(dto.Country, dto.AccountHomeCountry, StringComparison.OrdinalIgnoreCase))
            {
                if (decision != FraudDecision.Fraud)
                {
                    decision = FraudDecision.Suspicious;
                }
                reasons.Add("Transaction originated outside account home country.");
            }

            if (dto.Amount > 10000)
            {
                decision = FraudDecision.Fraud;
                reasons.Add("Amount exceeds fraud threshold.");
            }

            if (BlockedCountries.Contains(dto.Country))
            {
                decision = FraudDecision.Fraud;
                reasons.Add("Transaction originated from blocked country.");
            }

            var oneMinuteAgo = dto.OccuredAt.AddMinutes(-1);

            var recentCount = await _context.Transactions.CountAsync(t =>
            t.AccountId == dto.AccountId &&
            t.OccuredAt >= oneMinuteAgo &&
            t.OccuredAt <= dto.OccuredAt);

            if (recentCount >= 3)
            {
                decision = FraudDecision.Fraud;
                reasons.Add("Too many recent transactions for this account.");
            }

            var transaction = new Transaction
            {
                AccountId = dto.AccountId,
                Amount = dto.Amount,
                Country = dto.Country,
                Merchant = dto.Merchant,
                OccuredAt = dto.OccuredAt,
                Decision = decision,
                Reason = reasons.Count > 0 ? string.Join(" ", reasons) : "Transaction approved.",
                CreatedAt = DateTime.UtcNow
            };

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();

            return transaction;
        }

        public async Task<List<Transaction>> GetAllTransactionsAsync()
        {
            return await _context.Transactions
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
            ;
        }

        public async Task<Transaction?> GetTransactionByIdAsync(int id)
        {
            return await _context.Transactions.FindAsync(id);
        }

    }
}