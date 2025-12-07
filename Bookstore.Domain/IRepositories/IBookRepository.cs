using Bookstore.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Domain.IRepositories
{
    public interface IBookRepository
    {
        Task<List<Book>> GetAllAsync();
        Task<Book> GetByIdAsync(Guid id);
        Task CreateAsync(Book book);
        Task UpdateAsync(Book book);
        Task DeleteAsync(Guid id);
    }
}
