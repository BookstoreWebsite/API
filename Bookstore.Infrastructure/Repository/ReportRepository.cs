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
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;
        private readonly IBookRepository _bookRepository;

        public ReportRepository(AppDbContext context, IBookRepository bookRepository) 
        {
            _context = context;
            _bookRepository = bookRepository;
        }

        public async Task<Comment> GetCommentByIdAsync(Guid commentId)
        {
            return await _context.Comments.Include(c => c.Reader).FirstOrDefaultAsync(c => c.Id == commentId);
        }

        public async Task RemoveCommentAsync(Comment comment)
        {
            _context.Update(comment);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveReviewAsync(Guid reviewId)
        {
            var review = await _context.Reviews
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == reviewId);

            if (review == null)
                return;

            var bookId = review.BookId;

            _context.Reviews.Remove(new Review { Id = reviewId });
            await _context.SaveChangesAsync();

            var avg = await _context.Reviews
                .Where(r => r.BookId == bookId)
                .Select(r => (double?)r.Rating)
                .AverageAsync();

            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == bookId);
            if (book == null) return;

            book.Rating = avg ?? 0.0;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveReportAsync(Report report) 
        {
            _context.Remove(report);
            await _context.SaveChangesAsync();
        }

        public async Task<Report> GetReportByIdAsync(Guid reportId) 
        {
            return await _context.Reports.FindAsync(reportId);
        }
    }
}
