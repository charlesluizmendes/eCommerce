namespace Payment.Domain.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        Task<Models.Payment> GetByBasketIdAsync(int basketId);
        Task InsertAsync(Models.Payment payment);
    }
}
