using Payment.Domain.Interfaces.Repositories;
using Payment.Domain.Interfaces.Services;
using Payment.Domain.Models;

namespace Payment.Domain.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _repository;

        public TransactionService(ITransactionRepository repository)
        {
            _repository = repository;
        }

        public Task<Transaction> GetByPaymentIdAsync(int paymentId)
        {
            return _repository.GetByPaymentIdAsync(paymentId);
        }

        public async Task InsertAsync(Transaction transaction)
        {
            // Toda Transação é criada como Status 'Pendente'
            transaction.StatusId = 1;

            await _repository.InsertAsync(transaction); 
        }

        public async Task UpdateAsync(Transaction transaction)
        {           
            await _repository.UpdateAsync(transaction);
        }
    }
}
