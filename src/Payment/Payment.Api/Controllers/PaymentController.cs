using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Payment.Application.Interface;
using Payment.Application.ViewModels;

namespace Payment.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentAppService _appService;

        public PaymentController(IPaymentAppService appService)
        {
            _appService = appService;
        }

        [HttpPost("Card")]
        public async Task<ActionResult> Card(CreatePaymentCardViewModel viewModel)
        {
            var payment = await _appService.CreatePaymentCardAsync(viewModel);

            if (!payment)
                return BadRequest();

            return Ok();
        }

        [HttpPost("Pix")]
        public async Task<ActionResult> Pix(CreatePaymentPixViewModel viewModel)
        {
            var payment = await _appService.CreatePaymentPixAsync(viewModel);

            if (!payment)
                return BadRequest();

            return Ok();
        }
    }
}
