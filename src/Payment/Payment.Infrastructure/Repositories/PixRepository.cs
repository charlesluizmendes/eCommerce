using Payment.Domain.Interfaces.Repositories;
using Payment.Domain.Models;
using Payment.Infrastructure.Context;

namespace Payment.Infrastructure.Repositories
{
    public class PixRepository : IPixRepository
    {
        private readonly PaymentContext _context;

        public PixRepository(PaymentContext context)
        {
            _context = context;
        }

        public async Task InsertAsync(Pix pix)
        {
            try
            {
                await _context.Pix.AddAsync(pix);
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
