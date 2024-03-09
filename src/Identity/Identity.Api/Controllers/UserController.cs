using AutoMapper;
using Identity.Application.ViewModels;
using Identity.Domain.Interfaces.Services;
using Identity.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public UserController(IMapper mapper, IUserService userService)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserViewModel>> Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var user = await _userService.GetByIdAsync(id);

            return Ok(_mapper.Map<UserViewModel>(user));
        }

        [HttpPost]
        public async Task<ActionResult> Post(CreateUserViewModel viewModel)
        {
            await _userService.InsertAsync(_mapper.Map<User>(viewModel));
            
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> Put(UpdateUserViewModel viewModel)
        {
            await _userService.UpdateAsync(_mapper.Map<User>(viewModel));

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var user = await _userService.GetByIdAsync(id);

            await _userService.DeleteAsync(user);

            return Ok();
        }
    }
}
