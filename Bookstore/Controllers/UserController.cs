using Bookstore.Application.DTO;
using Bookstore.Application.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(Guid id) 
        {
            var user = await _userService.GetByIdAsync(id);
            if(user == null) return NotFound();

            return Ok(user);
        }
    }
}
