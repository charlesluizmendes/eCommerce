using Basket.Domain.Interfaces.Repositories;
using Basket.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Basket.Infrastructure.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly BasketContext _context;

        public BasketRepository(BasketContext context)
        {
            _context = context;
        }

        public async Task<Domain.Models.Basket> GetByIdAsync(int id)
        {
            return await _context.Basket
                    .Include(x => x.Items.Where(item => item.Active))
                    .Where(basket => basket.Id == id && basket.Active)
                    .FirstOrDefaultAsync();
        }

        public async Task<Domain.Models.Basket> GetByUserIdAsync(string userId)
        {
            try
            {
                return await _context.Basket
                    .Include(x => x.Items.Where(item => item.Active))
                    .Where(basket => basket.UserId == userId && basket.Active)
                    .FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddAsync(Domain.Models.Basket basket)
        {
            try
            {
                await _context.Basket.AddAsync(basket);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(Domain.Models.Basket basket)
        {
            try
            {
                _context.Basket.Update(basket);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
