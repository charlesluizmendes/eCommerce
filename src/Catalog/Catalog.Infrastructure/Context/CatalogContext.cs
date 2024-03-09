using Catalog.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Context
{
    public class CatalogContext : DbContext, IDisposable
    {
        public virtual DbSet<Product> Product { get; set; }

        public CatalogContext(DbContextOptions<CatalogContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Product>().HasData(
                new Product
                { 
                    Id = 1,
                    Name = "Biscoito Trakinas",
                    Description = "Biscoito Recheado Sabor Morango",
                    Price = 2.00
                },
                new Product
                {
                    Id = 2,
                    Name = "Refrigerante Pepsi",
                    Description = "Refrigerante Sabor Cola",
                    Price = 5.00
                });
        } 
    }
}
