using Bookstore.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Domain.IRepositories
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(Guid id);
        Task<List<User>> GetAllAsync();
        Task Follow(Guid followerId, Guid followingId);
        Task<List<User>> GetBySearchQueryAsync(string query);
        Task<List<User>> GetFollowingAsync(Guid id);
    }
}
