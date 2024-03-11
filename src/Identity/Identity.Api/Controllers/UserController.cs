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

        [HttpGet("Get/{id}")]
        public async Task<ActionResult<UserViewModel>> Get(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest();
            }

            var user = await _userService.GetByIdAsync(id);

            return Ok(_mapper.Map<UserViewModel>(user));
        }

        [HttpPost("Add")]
        public async Task<ActionResult> Add(AddUserViewModel viewModel)
        {
            var add = await _userService.InsertAsync(_mapper.Map<User>(viewModel));
            
            if (add)
                return BadRequest();

            return Ok();
        }

        [HttpPut("Alter")]
        public async Task<ActionResult> Alter(AlterUserViewModel viewModel)
        {
            var alter = await _userService.UpdateAsync(_mapper.Map<User>(viewModel));

            if (!alter)
                return BadRequest();

            return Ok();
        }

        [HttpDelete("Remove/{id}")]
        public async Task<ActionResult> Remove(string id)
        {
            var user = await _userService.GetByIdAsync(id);

            if (user == null)
                return NotFound();

            var delete = await _userService.DeleteAsync(user);

            if (delete)
                return BadRequest();

            return Ok();
        }
    }
}
