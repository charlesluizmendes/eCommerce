using AutoMapper;
using Identity.Application.ViewModels;
using Identity.Domain.Interfaces.Services;
using Identity.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccessTokenController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IAccessTokenService _accessTokenService;

        public AccessTokenController(IMapper mapper,
            IAccessTokenService accessTokenService)
        {
            _mapper = mapper;
            _accessTokenService = accessTokenService;
        }

        [HttpPost]
        public async Task<ActionResult<AccessTokenViewModel>> Post(CreateAccessTokenViewModel viewModel)
        {
            var token = await _accessTokenService.CreateAcessTokenByUserAsync(_mapper.Map<User>(viewModel));

            if (token == null)            
                return Unauthorized();
            
            return Ok(_mapper.Map<AccessTokenViewModel>(token));            
        }
    }
}
