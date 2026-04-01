

using FraudDetectionApi.Dtos;
using FraudDetectionApi.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FraudDetectionApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransaction([FromBody] CreateTransactionDto dto)
        {
            try
            {
                var transaction = await _transactionService.CreateTransactionAsync(dto);
                return CreatedAtAction(nameof(GetTransactionById), new { id = transaction.Id }, transaction);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred while creating the transaction." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactions()
        {
            var transactions = await _transactionService.GetAllTransactionsAsync();
            return Ok(transactions);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var transaction = await _transactionService.GetTransactionByIdAsync(id);

            if (transaction is null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            try
            {

                var deleted = await _transactionService.DeleteTransactionAsync(id);

                if (!deleted)
                {
                    return NotFound(new { message = "Transaction not found." });
                }

                return NoContent();

            }
            catch
            {
                return StatusCode(500, new { message = "An unexpected error occurred while deleting the transaction." });
            }


        }
    }
}