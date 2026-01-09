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
        Task<BookDto> GetByIdAsync(Guid bookId);
        Task<bool> CreateAsync(BookDto bookDto, List<Guid> genreIds);
        Task<bool> UpdateAsync(Guid bookId, BookDto bookDto);
        Task<bool> DeleteAsync(Guid bookId);
        Task<List<BookDto>> GetAllGenreBooksAsync(Guid bookId);
        Task<List<ReviewDto>> GetAllBookReviewsAsync(Guid bookId);
        Task<ReviewDto> GetReviewByIdAsync(Guid reviewId);
        Task<bool> CreateReviewAsync(ReviewDto reviewDto, Guid bookId, Guid ReaderId);
        Task<bool> CreateCommentAsync(CommentDto commentDto, Guid readerId, Guid reviewId, Guid bookId, Guid? parentCommentId = null);
        Task<List<CommentDto>> GetAllReviewCommentsAsync(Guid reviewId);
        Task<List<CommentDto>> GetAllCommentRepliesAsync(Guid parentCommentId);
        Task<bool> CreateReportAsync(ReportDto reportDto, Guid readerId, Guid parentId);
        Task<List<ReportDto>> GetAllReportsAsync();
        Task<bool> AddToWishedAsync(Guid readerId, Guid bookId);
        Task<bool> AddToReadAsync(Guid readerId, Guid bookId);
        Task<List<BookDto>> GetAllWishedAsync(Guid readerId);
        Task<List<BookDto>> GetAllReadAsync(Guid readerId);
    }
}
