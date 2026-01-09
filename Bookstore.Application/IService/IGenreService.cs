using Bookstore.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Application.IService
{
    public interface IGenreService
    {
        Task<List<GenreDto>> GetAllAsync();
        Task<GenreDto> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(GenreFormsDto genreDto);
        Task<bool> UpdateAsync(Guid id, GenreFormsDto genreDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
