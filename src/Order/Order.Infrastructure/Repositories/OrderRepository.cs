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

        public async Task InsertAsync(Domain.Models.Order order)
        {
            try
            {
                // Verifica se a Order ja foi Criada
                var order_ = _context.Order
                    .Include(b => b.Basket)
                    .Include(b => b.Basket.Items)
                    .FirstOrDefault(b => b.Basket.Id == order.Basket.Id);

                if (order_ == null) 
                {
                    _context.Order.Add(order);
                    _context.Basket.Add(order.Basket);
                    _context.Item.AddRange(order.Basket.Items);

                    await _context.SaveChangesAsync();
                }               
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
