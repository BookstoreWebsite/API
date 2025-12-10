using Bookstore.Domain.IRepositories;
using Bookstore.Domain.Model;
using Bookstore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Infrastructure.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<List<User>> GetAllAsync() 
        {
            return _context.Users.ToList();
        }

        public async Task Follow(Guid followerId, Guid followingId)
        {
            var user = await GetByIdAsync(followerId);
            var targetUser = await GetByIdAsync(followingId);

            user.Following.Add(targetUser);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetBySearchQueryAsync(string query) 
        {
            var users = await GetAllAsync();
            string lowerCaseQuery = query.ToLower(); 
            List<User> result = new List<User>();

            foreach (var user in users) 
            {
                bool containsQuery = user.Username.ToLower().Contains(lowerCaseQuery)
                    || user.FirstName.ToLower().Contains(lowerCaseQuery)
                    || user.LastName.ToLower().Contains(lowerCaseQuery);

                if (containsQuery)
                    result.Add(user);
            }

            return result;
        }

        public async Task<List<User>> GetFollowingAsync(Guid id)
        {
            var following = await _context.Users
                .Where(u => u.Id == id)
                .SelectMany(u => u.Following)
                .ToListAsync();
            return following;
        }
    }
}
