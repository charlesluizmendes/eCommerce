namespace Payment.Domain.Interfaces.Services
{
    public interface IPaymentService
    {
        Task CreatePaymentAsync(Models.Payment payment);
    }
}
