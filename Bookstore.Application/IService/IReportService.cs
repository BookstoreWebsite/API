using Bookstore.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Application.IService
{
    public interface IReportService
    {
        Task<CommentDto> GetCommentByIdAsync(Guid commentId);
        Task<bool> RemoveCommentAsync(Guid commentId);
        Task<bool> RemoveReviewAsync(Guid commentId);
        Task<bool> RemoveReportAsync(Guid reportId);
    }
}
