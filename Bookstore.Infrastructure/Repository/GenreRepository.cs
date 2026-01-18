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
    public class GenreRepository : IGenreRepository
    {
        private readonly AppDbContext _context;

        public GenreRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Genre genre)
        {
            if (genre != null)
                _context.Genres.Add(genre);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var genre = await GetByIdAsync(id);
            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Genre>> GetAllAsync()
        {
            return await _context.Genres.ToListAsync();
        }

        public async Task<Genre> GetByIdAsync(Guid id)
        {
            return await _context.Genres.FindAsync(id);
        }

        public async Task UpdateAsync(Genre genre)
        {
            _context.Genres.Update(genre);
            await _context.SaveChangesAsync();
        }
        public async Task AddGenresToFavoritesAsync(Guid readerId, List<Genre> genres) 
        {
            var user = await _context.Users
                .Include(u => u.FavoriteGenres)
                .FirstOrDefaultAsync(u => u.Id == readerId);

            foreach (var genre in genres) 
            {
                user.FavoriteGenres.Add(genre);
            }
            await _context.SaveChangesAsync();
        }
    }
}
