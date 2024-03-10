using AutoMapper;
using Catalog.Application.ViewModels;
using Catalog.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IProductService _productService;

        public ProductController(IMapper mapper,
            IProductService _roductService)
        {
            _mapper = mapper;
            _productService = _roductService;
        }

        [HttpGet("Get")]
        public async Task<ActionResult<ProductViewModel>> Get()
        {
            var products = await _productService.GetListAsync();

            return Ok(_mapper.Map<List<ProductViewModel>>(products));
        }

        [HttpGet("Get/{id}")]
        public async Task<ActionResult<ProductViewModel>> Get(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            return Ok(_mapper.Map<ProductViewModel>(product));
        }
    }
}
