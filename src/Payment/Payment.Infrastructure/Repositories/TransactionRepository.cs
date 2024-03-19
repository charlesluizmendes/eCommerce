using Microsoft.EntityFrameworkCore;
using Payment.Domain.Interfaces.Repositories;
using Payment.Domain.Models;
using Payment.Infrastructure.Context;

namespace Payment.Infrastructure.Repositories
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly PaymentContext _context;

        public TransactionRepository(PaymentContext context)
        {
            _context = context;
        }

        public async Task<Transaction> GetByPaymentIdAsync(int paymentId)
        {
            try
            {
                return await _context.Transaction
                    .Where(x => x.PaymentId == paymentId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task InsertAsync(Transaction transaction)
        {
            try
            {
                await _context.Transaction.AddAsync(transaction);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(Transaction transaction)
        {
            try
            {
                _context.Update(transaction);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
