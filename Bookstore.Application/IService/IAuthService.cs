using Bookstore.Application.DTO;
using Bookstore.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Application.IService
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegistrationDto request);
        Task<TokenResponseDto?> LoginAsync(LoginUserDto request);
        Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request);
    }
}
