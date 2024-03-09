using Payment.Application.Interface;
using Payment.Application.ViewModels;
using Payment.Domain.Interfaces.Services;
using Payment.Domain.Models;

namespace Payment.Application.Service
{
    public class PaymentAppService : IPaymentAppService
    {
        private readonly IPaymentService _service;

        public PaymentAppService(IPaymentService service)
        {
            _service = service;
        }

        public async Task<bool> CreatePaymentCardAsync(CreatePaymentCardViewModel viewModel)
        {
            var payment = new Domain.Models.Payment()
            {
                UserId = viewModel.UserId,
                Card = new Card()
                {
                    ClientName = viewModel.ClientName,
                    Number = viewModel.Number,
                    DateValidate = viewModel.DateValidate,
                    SecurityCode = viewModel.SecurityCode
                }
            };

            return await _service.InsertAsync(payment);
        }

        public async Task<bool> CreatePaymentPixAsync(CreatePaymentPixViewModel viewModel)
        {
            var payment = new Domain.Models.Payment()
            {
                UserId = viewModel.UserId,
                Pix = new Pix() 
                { 
                    Key = viewModel.Key
                }
            };

            return await _service.InsertAsync(payment);
        }
    }
}
