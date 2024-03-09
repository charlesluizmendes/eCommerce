using Basket.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Basket.Infrastructure.Context
{
    public class BasketContext : DbContext, IDisposable
    {
        public virtual DbSet<Domain.Models.Basket> Basket { get; set; }
        public virtual DbSet<Item> Item { get; set; }

        public BasketContext(DbContextOptions<BasketContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
