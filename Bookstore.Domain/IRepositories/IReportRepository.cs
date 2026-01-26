using Bookstore.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Domain.IRepositories
{
    public interface IReportRepository
    {
        Task<Comment> GetCommentByIdAsync(Guid commentId);
        Task RemoveCommentAsync(Comment comment);
        Task RemoveReviewAsync(Guid reviewId);
        Task RemoveReportAsync(Report report);
        Task<Report> GetReportByIdAsync(Guid reportId);
    }
}
