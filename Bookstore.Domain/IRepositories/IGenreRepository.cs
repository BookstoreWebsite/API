using Bookstore.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Domain.IRepositories
{
    public interface IGenreRepository
    {
        Task<List<Genre>> GetAllAsync();
        Task<Genre> GetByIdAsync(Guid id);
        Task CreateAsync(Genre genre);
        Task UpdateAsync(Genre genre);
        Task DeleteAsync(Guid id);
    }
}
