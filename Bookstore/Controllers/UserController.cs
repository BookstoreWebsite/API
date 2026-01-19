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

        [HttpPost("follow/{followerId}/{followingId}")]
        public async Task<IActionResult> Follow( Guid followerId, Guid followingId) 
        {
            var result = await _service.Follow(followerId, followingId);

            if (!result)
            {
                return BadRequest();
            }
            return Ok(new { message = "User successfully followed!" });
        }

        [HttpDelete("unfollow/{followerId}/{followingId}")]
        public async Task<IActionResult> Unfollow(Guid followerId, Guid followingId)
        {
            var result = await _service.Unfollow(followerId, followingId);

            if (!result)
            {
                return BadRequest();
            }
            return Ok(new { message = "User successfully unfollowed!" });
        }

        [HttpGet("getFollowing/{id}")]
        public async Task<ActionResult<List<FollowDto>>> GetFollowing(Guid id) 
        {
            var result = await _service.GetFollowingAsync(id);
            return Ok(result);
        }

        [HttpGet("getFollowers/{id}")]
        public async Task<ActionResult<List<FollowDto>>> GetFollowers(Guid id)
        {
            var result = await _service.GetFollowersAsync(id);
            return Ok(result);  
        }

        public class EditBioRequest
        {
            public string Text { get; set; } = "";
        }

        [HttpPatch("editBio/{id}")]
        public async Task<IActionResult> EditBio(Guid id, [FromBody] EditBioRequest request)
        {
            var result = await _service.EditBioAsync(id, request.Text);

            if (!result) return BadRequest();

            return Ok(new { message = "Bio updated successfully" });
        }

    }
}
