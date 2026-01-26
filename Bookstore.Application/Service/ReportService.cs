using Bookstore.Application.DTO;
using Bookstore.Application.IService;
using Bookstore.Domain.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookstore.Application.Service
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _repository;

        public ReportService(IReportRepository repository) 
        {
            _repository = repository;
        }

        public async Task<CommentDto> GetCommentByIdAsync(Guid commentId)
        {
            var comment = await _repository.GetCommentByIdAsync(commentId);

            var commentDto = new CommentDto
            {
                Id = comment.Id,
                Text = comment.Text,
                Username = comment.Reader.Username,
                ProfilePicture = comment.Reader.ProfilePictureUrl,
            };

            return commentDto;
        }

        public async Task<bool> RemoveCommentAsync(Guid commentId)
        {
            var comment = await _repository.GetCommentByIdAsync(commentId);
            comment.Text = "[removed]";
            comment.IsRemoved = true;
            await _repository.RemoveCommentAsync(comment);
            return true;
        }

        public async Task<bool> RemoveReviewAsync(Guid reviewId) 
        {
            await _repository.RemoveReviewAsync(reviewId);
            return true;
        }

        public async Task<bool> RemoveReportAsync(Guid reportId) 
        {
            var report = await _repository.GetReportByIdAsync(reportId);
            await _repository.RemoveReportAsync(report);
            return true;
        }
    }
}
