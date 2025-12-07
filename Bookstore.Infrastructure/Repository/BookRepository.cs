using Bookstore.Domain.IRepositories;
using Bookstore.Domain.Model;
using Bookstore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Infrastructure.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;
        
        public BookRepository(AppDbContext context) 
        {
            _context = context;
        }

        public async Task CreateAsync(Book book)
        {
            if(book != null)
                await _context.Books.AddAsync(book);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var book = await GetByIdAsync(id);
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Book>> GetAllAsync()
        {
            return await _context.Books.ToListAsync(); 
        }

        public async Task<Book> GetByIdAsync(Guid id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task UpdateAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
        }
    }
}
