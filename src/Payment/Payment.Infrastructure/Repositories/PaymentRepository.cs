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
                    .FirstOrDefaultAsync(x => x.BasketId == basketId);
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

                if (payment.Card != null)
                    await _context.Card.AddAsync(payment.Card);

                if (payment.Pix != null)
                    await _context.Pix.AddAsync(payment.Pix);

                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
