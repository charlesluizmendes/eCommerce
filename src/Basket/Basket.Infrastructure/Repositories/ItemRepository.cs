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
                        .Where(x => x.Id == id
                                 && x.Active == true)
                        .FirstOrDefaultAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task AddAsync(Item item)
        {
            await _context.Item.AddAsync(item);
        }

        public void Remove(Item item)
        {
            try
            {
                item.Active = false;
                _context.Item.Update(item);
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
