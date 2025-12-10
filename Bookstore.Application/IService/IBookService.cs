using Bookstore.Application.DTO;
using Bookstore.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Application.IService
{
    public interface IBookService
    {
        Task<List<BookDto>> GetAllAsync();
        Task<BookDto> GetByIdAsync(Guid id);
        Task<bool> CreateAsync(BookDto bookDto);
        Task<bool> UpdateAsync(Guid id, BookDto bookDto);
        Task<bool> DeleteAsync(Guid id);
    }
}
