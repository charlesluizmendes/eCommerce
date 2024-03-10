using AutoMapper;
using Basket.Application.ViewModels;
using Basket.Domain.Interfaces.Services;
using Basket.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Basket.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IItemService _itemService;

        public ItemController(IMapper mapper,
            IItemService itemService)
        {
            _mapper = mapper;
            _itemService = itemService;
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(AddItemViewModel viewModel)
        {
            await _itemService.AddToBasketAsync(_mapper.Map<Item>(viewModel));

            return Ok();
        }

        [HttpDelete("Remove/{id}")]
        public async Task<ActionResult> Remove(int id)
        {
            await _itemService.RemoveFromBasketAsync(id);

            return Ok();
        }
    }
}
