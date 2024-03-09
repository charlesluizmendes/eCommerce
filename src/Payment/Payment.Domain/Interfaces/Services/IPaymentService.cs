namespace Payment.Domain.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<bool> InsertAsync(Models.Payment payment);
    }
}
