
using FraudDetectionApi.Dtos;
using FraudDetectionApi.Models;

namespace FraudDetectionApi.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction> CreateTransactionAsync(CreateTransactionDto dto);
        Task<List<Transaction>> GetAllTransactionsAsync();
        Task<Transaction?> GetTransactionByIdAsync(int id);
    }
}