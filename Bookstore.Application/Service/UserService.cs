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
            var following = user.Following.ToList();
            var followers = user.Followers.ToList();
            var followingIds = ConvertFollowLists(following);
            var followerIds = ConvertFollowLists(followers);
            var genreDtos = ConvertGenresToDtos(user.FavoriteGenres);

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
                FollowerIds = followerIds,
                WishedBooksCount = user.Wished.Count,
                ReadBooksCount = user.Read.Count,
                FavoriteGenres = genreDtos
            };

            return userDto;
        }

        public async Task<List<UserDto>> GetAllAsync()
        {
            var users = await _repository.GetAllAsync();

            var userDtos = ConvertToDto(users);

            return userDtos;
        }


        public async Task<bool> Follow(Guid followerId, Guid followingId) 
        {
            await _repository.Follow(followerId, followingId);
            return true;
        }

        public async Task<bool> Unfollow(Guid followerId, Guid followingId)
        {
            await _repository.Follow(followerId, followingId);
            return true;
        }

        public async Task<List<UserDto>> GetBySearchQueryAsync(string query) 
        {
            var users = await _repository.GetBySearchQueryAsync(query);
            var userDtos = ConvertToDto(users);

            return userDtos;

        }

        private List<UserDto> ConvertToDto(List<User> users) 
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

        private List<Guid> ConvertFollowLists(ICollection<User> followList) 
        {
            var ids = new List<Guid>();

            foreach (var user in followList) 
            {
                ids.Add(user.Id);
            }

            return ids;
        }

        public async Task<List<FollowDto>> GetFollowingAsync(Guid id) 
        {
            var following = await _repository.GetFollowingAsync(id);
            var followingDto = new List<FollowDto>();

            foreach(var follow in following) 
            {
                var followDto = new FollowDto 
                {
                    Id = follow.Id,
                    Username = follow.Username,
                    FirstName = follow.FirstName,
                    LastName = follow.LastName,
                    ProfilePicture = follow.ProfilePictureUrl
                };

                followingDto.Add(followDto);
            }

            return followingDto;
        }

        public async Task<List<FollowDto>> GetFollowersAsync(Guid id)
        {
            var followers = await _repository.GetFollowersAsync(id);
            var followersDto = new List<FollowDto>();

            foreach (var follow in followers)
            {
                var followDto = new FollowDto
                {
                    Id = follow.Id,
                    Username = follow.Username,
                    FirstName = follow.FirstName,
                    LastName = follow.LastName,
                    ProfilePicture = follow.ProfilePictureUrl
                };

                followersDto.Add(followDto);
            }

            return followersDto;
        }

        private List<GenreDto> ConvertGenresToDtos(ICollection<Genre> genres) 
        {
            var dtos = new List<GenreDto>();
            foreach (var genre in genres) 
            {
                var dto = new GenreDto
                {
                    Id = genre.Id,
                    Name = genre.Name
                };
                dtos.Add(dto);
            }
            return dtos;
        }

        public async Task<bool> EditBioAsync(Guid id, string text) 
        {
            var user = await _repository.GetByIdAsync(id);
            user.ReaderBio = text;
            await _repository.EditBioAsync(user);
            return true;
        }
    }
}
