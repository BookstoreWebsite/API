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
        private readonly IUserRepository _userRepository;
        
        public BookRepository(AppDbContext context, IUserRepository userRepository) 
        {
            _context = context;
            _userRepository = userRepository;
        }

        public async Task CreateAsync(Book book, List<Guid> genreIds, decimal? price)
        {
            if (book != null)
            {
                var genres = await _context.Genres
                    .Where(g => genreIds.Contains(g.Id))
                    .ToListAsync();

                foreach (var genre in genres)
                {
                    book.Genres.Add(genre);
                }

                _context.Books.Add(book);

                var priceListEntry = new PriceListEntry
                {
                    ProductId = book.Id,
                    ValidFrom = DateTime.UtcNow,
                    Price = (decimal)price
                };

                _context.PriceListEntries.Add(priceListEntry);
            }
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

        public async Task<List<Book>> GetAllGenreBooksAsync(Guid genreId) 
        {
            return await _context.Books
                .Where(b => b.Genres.Any(g => g.Id == genreId))
                .ToListAsync();
        }

        public async Task CreateReviewAsync(Review review)
        {
            if (review == null) throw new ArgumentNullException(nameof(review));

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var avg = await _context.Reviews
                .Where(r => r.BookId == review.BookId)
                .AverageAsync(r => (double?)r.Rating);

            var book = await _context.Books.FirstAsync(b => b.Id == review.BookId);
            book.Rating = avg;
            await _context.SaveChangesAsync();
        }


        public async Task<List<Review>> GetAllBookReviewsAsync(Guid bookId)
        {
            return await _context.Reviews.Where(r => r.BookId == bookId).ToListAsync();
        }

        public async Task<Review> GetReviewByIdAsync(Guid reviewId)
        {
            return await _context.Reviews.FindAsync(reviewId);
        }

        public async Task CreateCommentAsync(Comment comment) 
        {
            if (comment != null)
            {
                _context.Comments.Add(comment);

                if (comment.ParentCommentId != null)
                {
                    var parentComment = await GetCommentById((Guid)comment.ParentCommentId);
                    parentComment.HasReplies = true;
                    _context.Comments.Update(parentComment);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<Comment> GetCommentById(Guid id) 
        {
            return await _context.Comments.FindAsync(id);
        }

        public async Task<List<Comment>> GetAllReviewCommentsAsync(Guid reviewId) 
        {
            var comments = await _context.Comments.
                Where(c => c.ReviewId == reviewId && c.ParentCommentId == null).
                Include(c => c.Reader).
                ToListAsync();
            return comments;
        }

        public async Task<List<Comment>> GetAllCommentRepliesAsync(Guid parentCommentId) 
        {
            var comments = await _context.Comments.
                Where(c => c.ParentCommentId == parentCommentId).
                Include(c => c.Reader).
                ToListAsync();
            return comments;
        }

        public async Task CreateReportAsync(Report report) 
        {
            if(report!= null)
                _context.Reports.Add(report);

            await _context.SaveChangesAsync();
        }

        public async Task<List<Report>> GetAllReportsAsync() 
        {
            var reports = await _context.Reports.ToListAsync();
            return reports;
        }

        public async Task AddToWishedAsync(Guid readerId, Guid bookId) 
        {
            var book = await GetByIdAsync(bookId);
            var user = await _userRepository.GetByIdAsync(readerId);

            user.Wished.Add(book);
            await _context.SaveChangesAsync(); 
        }

        public async Task AddToReadAsync(Guid readerId, Guid bookId)
        {
            var book = await GetByIdAsync(bookId);
            var user = await _userRepository.GetByIdAsync(readerId);

            user.Read.Add(book);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Book>> GetAllWishedAsync(Guid readerId)
        {
            var user = await _context.Users
                .Include(u => u.Wished)
                .FirstOrDefaultAsync(u => u.Id == readerId);

            if (user == null)
                return new List<Book>();

            return user.Wished.ToList();
        }

        public async Task<List<Book>> GetAllReadAsync(Guid readerId)
        {
            var user = await _context.Users
                .Include(u => u.Read)
                .FirstOrDefaultAsync(u => u.Id == readerId);

            if (user == null)
                return new List<Book>();

            return user.Read.ToList();
        }

    }
}
