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

        [HttpGet]
        public async Task<ActionResult<ProductViewModel>> Get()
        {
            var products = await _productService.GetListAsync();

            return Ok(_mapper.Map<List<ProductViewModel>>(products));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductViewModel>> Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var product = await _productService.GetByIdAsync(id);

            return Ok(_mapper.Map<ProductViewModel>(product));
        }
    }
}
