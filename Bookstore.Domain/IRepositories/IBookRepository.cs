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
        Task CreateAsync(Book book, List<Guid> genreIds, decimal? price);
        Task UpdateAsync(Book book);
        Task DeleteAsync(Guid bookId);
        Task<List<Book>> GetAllGenreBooksAsync(Guid genreId);
        Task CreateReviewAsync(Review review);
        Task<List<Review>> GetAllBookReviewsAsync(Guid bookId);
        Task<Review> GetReviewByIdAsync(Guid reviewId);
        Task CreateCommentAsync(Comment comment);
        Task<List<Comment>> GetAllReviewCommentsAsync(Guid reviewId);
        Task<List<Comment>> GetAllCommentRepliesAsync(Guid parentCommentId);
        Task<Comment> GetCommentById(Guid id);
        Task CreateReportAsync(Report report);
        Task<List<Report>> GetAllReportsAsync();
        Task AddToWishedAsync(Guid readerId, Guid bookId);
        Task AddToReadAsync(Guid readerId, Guid bookId);
        Task<List<Book>> GetAllWishedAsync(Guid readerId);
        Task<List<Book>> GetAllReadAsync(Guid readerId);
    }
}
