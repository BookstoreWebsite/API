using Bookstore.Application.DTO;
using Bookstore.Application.IService;
using Bookstore.Domain.IRepositories;
using Bookstore.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Application.Service
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<UserDto> GetByIdAsync(Guid id) 
        {
            var user = await _repository.GetByIdAsync(id);
            var following = await _repository.GetFollowingAsync(id);
            var followers = await _repository.GetFollowersAsync(id);
            var followingIds = convertFollowLists(following);
            var followerIds = convertFollowLists(followers);

            var userDto = new UserDto
            {
                Id = id,
                Email = user.Email,
                Username = user.Username,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                Type = user.Type,
                ProfilePictureUrl = user.ProfilePictureUrl,
                ReaderBio = user.ReaderBio,
                FollowingIds = followingIds,
                FollowerIds = followerIds
            };

            return userDto;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _repository.GetAllAsync();

            var userDtos = convertToDto(users);

            return userDtos;
        }


        public async Task<bool> Follow(Guid followerId, Guid followingId) 
        {
            await _repository.Follow(followerId, followingId);
            return true;
        }

        public async Task<List<UserDto>> GetBySearchQueryAsync(string query) 
        {
            var users = await _repository.GetBySearchQueryAsync(query);
            var userDtos = convertToDto(users);

            return userDtos;

        }

        private List<UserDto> convertToDto(List<User> users) 
        {
            var userDtos = new List<UserDto>();
            foreach (var user in users)
            {
                var userDto = new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Username = user.Username,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Type = user.Type,
                    ProfilePictureUrl = user.ProfilePictureUrl,
                    ReaderBio = user.ReaderBio
                };

                userDtos.Add(userDto);
            }

            return userDtos;
        }

        private List<Guid> convertFollowLists(ICollection<User> followList) 
        {
            var ids = new List<Guid>();

            foreach (var user in followList) 
            {
                ids.Add(user.Id);
            }

            return ids;
        }
    }
}
