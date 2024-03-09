using Payment.Application.ViewModels;

namespace Payment.Application.Interface
{
    public interface IPaymentAppService
    {
        Task<bool> CreatePaymentCardAsync(CreatePaymentCardViewModel viewModel);
        Task<bool> CreatePaymentPixAsync(CreatePaymentPixViewModel viewModel);
    }
}
