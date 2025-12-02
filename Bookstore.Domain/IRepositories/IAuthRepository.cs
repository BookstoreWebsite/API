using Bookstore.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Domain.IRepositories
{
    public interface IAuthRepository
    {
        Task<User?> RegisterAsync(User request);
        Task<(Guid, string, string)?> LoginAsync(string email, string password);
        Task<(string, string)?> RefreshTokensAsync(Guid userId, string refreshToken);
    }
}
