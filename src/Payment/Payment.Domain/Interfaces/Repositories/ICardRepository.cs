using Payment.Domain.Models;

namespace Payment.Domain.Interfaces.Repositories
{
    public interface ICardRepository
    {
        Task InsertAsync(Card card);
    }
}
