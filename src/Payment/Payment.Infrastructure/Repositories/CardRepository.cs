using Payment.Domain.Interfaces.Repositories;
using Payment.Domain.Models;
using Payment.Infrastructure.Context;

namespace Payment.Infrastructure.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly PaymentContext _context;

        public CardRepository(PaymentContext context)
        {
            _context = context;
        }

        public async Task InsertAsync(Card card)
        {
            try
            {
                await _context.Card.AddAsync(card);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
