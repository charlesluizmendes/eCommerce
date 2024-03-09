using Payment.Domain.Models;

namespace Payment.Domain.Interfaces.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction> GetByPaymentIdAsync(int paymentId);
        Task InsertAsync(Transaction transaction);
        Task UpdateAsync(Transaction transaction);
    }
}
