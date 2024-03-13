using Microsoft.EntityFrameworkCore;
using Order.Domain.Interfaces.Repositories;
using Order.Infrastructure.Context;

namespace Order.Infrastructure.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderContext _context;

        public OrderRepository(OrderContext context)
        {
            _context = context;
        }

        public async Task<Domain.Models.Order> GetByBasketIdAsync(int id)
        {
            try
            {
                return await _context.Order
                    .Include(b => b.Basket)
                    .Include(b => b.Basket.Items)
                    .FirstOrDefaultAsync(b => b.Basket.Id == id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task InsertAsync(Domain.Models.Order order)
        {
            try
            {
                await _context.Order.AddAsync(order);
                await _context.Basket.AddAsync(order.Basket);
                await _context.Item.AddRangeAsync(order.Basket.Items);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void UpdateAsync(Domain.Models.Order order)
        {
            try
            {
                _context.Update(order);
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
