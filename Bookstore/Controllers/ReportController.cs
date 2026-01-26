using Bookstore.Application.DTO;
using Bookstore.Application.IService;
using Bookstore.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _service;
        public ReportController(IReportService service) 
        {
            _service = service;
        }

        [HttpGet("{commentId}")]
        public async Task<ActionResult<CommentDto>> GetCommentById(Guid commentId) 
        {
            var commentDto = await _service.GetCommentByIdAsync(commentId);
            if(commentDto == null) 
            {
                return NotFound("Comment not found!");
            }
            return Ok(commentDto);
        }

        [HttpPut("removeComment/{commentId}")]
        public async Task<IActionResult> RemoveComment(Guid commentId) 
        {
            var result = await _service.RemoveCommentAsync(commentId);
            if (!result) 
            {
                return NotFound("Comment not found!");
            }
            return Ok(new {message = "Comment successfully removed!" });
        }

        [HttpDelete("removeReview/{reviewId}")]
        public async Task<IActionResult> RemoveReview(Guid reviewId)
        {
            var result = await _service.RemoveReviewAsync(reviewId);
            if (!result)
            {
                return NotFound("Review not found!");
            }
            return Ok(new { message = "Review successfully removed!" });
        }

        [HttpDelete("removeReport/{reportId}")]
        public async Task<IActionResult> RemoveReport(Guid reportId) 
        {
            var result = await _service.RemoveReportAsync(reportId);
            if (!result)
            {
                return NotFound("Report not found!");
            }
            return Ok(new { message = "Report successfully removed!" });
        }
    }
}
