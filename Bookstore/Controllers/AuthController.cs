using Bookstore.Application.DTO;
using Bookstore.Application.IService;
using Bookstore.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Bookstore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService authService)
        {
            _service = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> Register([FromBody] RegistrationDto request)
        {
            var result = await _service.RegisterAsync(request);
            if (!result)
                return BadRequest();

            return Ok(new { message = "Registration successful!" });
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login([FromBody] LoginUserDto  request)
        {
            var result = await _service.LoginAsync(request);
            if (result is null)
                return BadRequest("Invalid username or password");

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await _service.RefreshTokensAsync(request);
            if (result is null || result.AccessToken is null || result.RefreshToken is null)
                return Unauthorized("Invalid refresh token");
            return Ok(result);
        }

    }
}
