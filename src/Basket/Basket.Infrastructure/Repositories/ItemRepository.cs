using Basket.Domain.Interfaces.Repositories;
using Basket.Domain.Models;
using Basket.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Basket.Infrastructure.Repositories
{
    public class ItemRepository : IItemRepository
    {
        private readonly BasketContext _context;

        public ItemRepository(BasketContext context)
        {
            _context = context;
        }

        public async Task<Item> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Item
                        .Include(x => x.Basket)
                        .Where(item => item.Id == id && item.Active && item.Basket.Active)
                        .FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(Item item)
        {
            try
            {
                _context.Item.Update(item);
            }
            catch (Exception)
            {
                throw;
            }
        }  
    }
}
