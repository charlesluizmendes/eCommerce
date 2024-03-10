using AutoMapper;
using Basket.Application.ViewModels;
using Basket.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Basket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IBasketService _basketService;

        public BasketController(IMapper mapper,
            IBasketService basketService)
        {
            _mapper = mapper;
            _basketService = basketService;
        }

        [HttpGet("Get/{userId}")]
        public async Task<ActionResult<BasketViewModel>> Get(string userId)
        {
            var basket = await _basketService.GetByUserIdAsync(userId);

            return Ok(_mapper.Map<BasketViewModel>(basket));
        }

        [HttpDelete("Remove/{id}")]
        public async Task<ActionResult> Remove(int id)
        {
            await _basketService.RemoveAsync(id);

            return Ok();
        }
    }
}
