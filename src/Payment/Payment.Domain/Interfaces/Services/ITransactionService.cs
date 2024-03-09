using Payment.Domain.Models;

namespace Payment.Domain.Interfaces.Services
{
    public interface ITransactionService
    {
        Task<Transaction> GetByPaymentIdAsync(int paymentId);
        Task InsertAsync(Transaction transaction);
        Task UpdateAsync(Transaction transaction);
    }
}
