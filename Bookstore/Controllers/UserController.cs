using Bookstore.Application.DTO;
using Bookstore.Application.IService;
using Bookstore.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetAll()  
        {
            var userDtos = await _service.GetAllAsync();
            return Ok(userDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(Guid id) 
        {
            var userDto = await _service.GetByIdAsync(id);
            if(userDto == null) return NotFound();

            return Ok(userDto);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<UserDto>>> GetBySearchQuery([FromQuery] string query) 
        {
            var userDtos = await _service.GetBySearchQueryAsync(query);
            return Ok(userDtos);
        }

        [HttpPost("{followerId}/{followingId}/follow")]
        public async Task<IActionResult> Follow( Guid followerId, Guid followingId) 
        {
            var result = await _service.Follow(followerId, followingId);

            if (!result)
            {
                return BadRequest();
            }
            return Ok(new { message = "User successfully followed!" });
        }
    }
}
