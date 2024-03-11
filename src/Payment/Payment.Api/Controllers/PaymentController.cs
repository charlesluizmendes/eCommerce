using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.ViewModels;
using Payment.Domain.Interfaces.Services;

namespace Payment.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPaymentService _paymentService;

        public PaymentController(IMapper mapper,
            IPaymentService paymentService )
        {
            _mapper = mapper;
            _paymentService = paymentService;
        }

        [HttpPost("Card")]
        public async Task<ActionResult> Card(CreatePaymentCardViewModel viewModel)
        {
            var payment = await _paymentService.CreatePaymentAsync(_mapper.Map<Domain.Models.Payment>(viewModel));

            if (!payment)
                return BadRequest();

            return Ok();
        }

        [HttpPost("Pix")]
        public async Task<ActionResult> Pix(CreatePaymentPixViewModel viewModel)
        {
            var payment = await _paymentService.CreatePaymentAsync(_mapper.Map<Domain.Models.Payment>(viewModel));

            if (!payment)
                return BadRequest();

            return Ok();
        }
    }
}
