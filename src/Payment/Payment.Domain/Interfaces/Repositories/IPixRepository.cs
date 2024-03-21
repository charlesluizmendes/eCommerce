using Payment.Domain.Models;

namespace Payment.Domain.Interfaces.Repositories
{
    public interface IPixRepository
    {
        Task InsertAsync(Pix card);
    }
}
