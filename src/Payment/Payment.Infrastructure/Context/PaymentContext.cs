using Microsoft.EntityFrameworkCore;
using Payment.Domain.Models;

namespace Payment.Infrastructure.Context
{
    public class PaymentContext : DbContext, IDisposable
    {
        public virtual DbSet<Domain.Models.Payment> Payment { get; set; }
        public virtual DbSet<Transaction> Transaction { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<Card> Card { get; set; }
        public virtual DbSet<Pix> Pix { get; set; }

        public PaymentContext(DbContextOptions<PaymentContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Status>().HasData(
                new Status
                {
                    Id = 1,
                    Name = "Pendente"                    
                },
                new Status
                {
                    Id = 2,
                    Name = "Aprovada"                   
                },
                new Status
                {
                    Id = 3,
                    Name = "Cancelada"
                });
        }
    }
}
