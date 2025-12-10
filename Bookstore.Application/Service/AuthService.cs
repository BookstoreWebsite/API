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

        public async Task<User?> RegisterAsync(RegistrationDto request)
        {
            var user = new User();


            var passwordHash = new PasswordHasher<User>()
                .HashPassword(user, request.HashedPassword);
            user.Email = request.Email;
            user.Username = request.Username;
            user.HashedPassword = passwordHash;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            user.Type = UserType.Reader;
            return await _authRepository.RegisterAsync(user);
        }
    }
}
