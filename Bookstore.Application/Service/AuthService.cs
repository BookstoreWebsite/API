using Bookstore.Application.DTO;
using Bookstore.Application.IService;
using Bookstore.Domain.IRepositories;
using Bookstore.Domain.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Application.Services
{
    public class AuthService : IAuthService
    {
        private IAuthRepository _authRepository;
        public AuthService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<TokenResponseDto?> LoginAsync(LoginUserDto request)
        {
            var result = await _authRepository.LoginAsync(request.Email, request.Password);
            if (result is null)
                return null;

            return new TokenResponseDto
            {
                userId = result.Value.Item1,
                AccessToken = result.Value.Item2,
                RefreshToken = result.Value.Item3
            };
        }

        public async Task<TokenResponseDto?> RefreshTokensAsync(RefreshTokenRequestDto request)
        {
            var result = await _authRepository.RefreshTokensAsync(request.UserId, request.RefreshToken);
            if (result is null)
                return null;

            return new TokenResponseDto
            {
                AccessToken = result.Value.Item1,
                RefreshToken = result.Value.Item2
            };
        }

        public async Task<bool> RegisterAsync(RegistrationDto request)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),               
                Email = request.Email,
                Username = request.Username,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Type = UserType.Reader
            };

            var passwordHash = new PasswordHasher<User>()
                .HashPassword(user, request.Password); 

            user.HashedPassword = passwordHash;

            var shoppingCart = new ShoppingCart
            {
                UserId = user.Id,                  
                User = user
            };

            user.ShoppingCart = shoppingCart;

            await _authRepository.RegisterAsync(user);
            return true;
        }
    }
}
