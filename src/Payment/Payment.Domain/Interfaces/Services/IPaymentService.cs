namespace Payment.Domain.Interfaces.Services
{
    public interface IPaymentService
    {
        Task<bool> CreatePaymentAsync(Models.Payment payment);
    }
}
