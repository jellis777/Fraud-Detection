
using FraudDetectionApi.Dtos;
using FraudDetectionApi.Models;

namespace FraudDetectionApi.Interfaces
{
    public interface ITransactionService
    {
        Task<TransactionResponseDto> CreateTransactionAsync(CreateTransactionDto dto);
        Task<List<TransactionResponseDto>> GetAllTransactionsAsync();
        Task<TransactionResponseDto?> GetTransactionByIdAsync(int id);
        Task<bool> DeleteTransactionAsync(int id);
    }
}