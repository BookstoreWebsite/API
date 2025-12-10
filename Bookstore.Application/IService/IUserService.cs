using Bookstore.Application.DTO;
using Bookstore.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Application.IService
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(Guid id);
        Task<List<UserDto>> GetAllAsync();
        Task<bool> Follow(Guid followerId, Guid followingId);
        Task<List<UserDto>> GetBySearchQueryAsync(string query);
    }
}
