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
    public class GenreService : IGenreService
    {
        private readonly IGenreRepository _repository;

        public GenreService(IGenreRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> CreateAsync(GenreFormsDto genreDto)
        {
            var newGenre = new Genre()
            {
                Name = genreDto.Name,
            };

            await _repository.CreateAsync(newGenre);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
            return true;
        }

        public async Task<List<GenreDto>> GetAllAsync()
        {
            var genres = await _repository.GetAllAsync();

            var genreDtos = new List<GenreDto>();

            foreach (var genre in genres)
            {
                var genreDto = new GenreDto
                {
                    Id = genre.Id,
                    Name = genre.Name
                };

                genreDtos.Add(genreDto);
            }
            return genreDtos;
        }

        public async Task<GenreDto> GetByIdAsync(Guid id)
        {
            var genre = await _repository.GetByIdAsync(id);

            var genreDto = new GenreDto
            {
                Id = genre.Id,
                Name = genre.Name
            };

            return genreDto;
        }

        public async Task<bool> UpdateAsync(Guid id, GenreFormsDto genreDto)
        {
            var oldGenre = await _repository.GetByIdAsync(id);

            oldGenre.Name = genreDto.Name;

            await _repository.UpdateAsync(oldGenre);
            return true;
        }
    }
}
