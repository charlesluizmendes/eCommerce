using Microsoft.EntityFrameworkCore;
using Payment.Domain.Interfaces.Repositories;
using Payment.Infrastructure.Context;

namespace Payment.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly PaymentContext _context;

        public PaymentRepository(PaymentContext context)
        {
            _context = context;
        }

        public async Task<Domain.Models.Payment> GetByBasketIdAsync(int basketId)
        {
            try
            {
                return await _context.Payment
                    .Where(x => x.BasketId == basketId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task InsertAsync(Domain.Models.Payment payment)
        {
            try
            {
                await _context.Payment.AddAsync(payment);
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
