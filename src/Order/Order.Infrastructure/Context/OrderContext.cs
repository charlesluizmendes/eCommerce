using Microsoft.EntityFrameworkCore;
using Order.Domain.Models;

namespace Order.Infrastructure.Context
{
    public class OrderContext : DbContext, IDisposable
    {
        public virtual DbSet<Domain.Models.Order> Order { get; set; }
        public virtual DbSet<Basket> Basket { get; set; }
        public virtual DbSet<Item> Item { get; set; }

        public OrderContext(DbContextOptions<OrderContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}