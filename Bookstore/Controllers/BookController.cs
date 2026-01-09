using Bookstore.Application.DTO;
using Bookstore.Application.IService;
using Bookstore.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Bookstore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _service;
        public BookController(IBookService bookService)
        {
            _service = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<List<BookDto>>> GetAll()
        {
            var bookDtos = await _service.GetAllAsync();
            return Ok(bookDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetById(Guid id) 
        {
            var bookDto = await _service.GetByIdAsync(id);
            if(bookDto != null) 
            {
                return Ok(bookDto);
            }

            return NotFound(new {message = "Book not found!" });
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody]CreateBookRequestDto requestDto) 
        {
            var result = await _service.CreateAsync(requestDto.bookDto, requestDto.genreIds);

            if (!result) 
            {
                return BadRequest();
            }
            return Ok(new { message = "Book successfully created!" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody]BookDto bookDto) 
        {
            var result = await _service.UpdateAsync(id, bookDto);

            if (!result) 
            {
                return NotFound(new { message = "Book not found!" });
            }

            return Ok(new {message = "Book successully updated!" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id) 
        {
            var result = await _service.DeleteAsync(id);

            if (!result) 
            {
                return NotFound(new { message = "Book not found" });
            }

            return Ok(new { message = "Book successfully deleted!" });
        }

        [HttpGet("genreBooks/{id}")]
        public async Task<ActionResult<BookDto>> GetAllGenreBooks(Guid id) 
        {
            var bookDtos = await _service.GetAllGenreBooksAsync(id);
            return Ok(bookDtos);
        }

        [HttpPost("createReview/{bookId}/{readerId}")]
        public async Task<IActionResult> CreateReview([FromBody] ReviewDto reviewDto, Guid bookId, Guid readerId) 
        {
            var result = await _service.CreateReviewAsync(reviewDto, bookId, readerId);

            if (!result)
            {
                return BadRequest();
            }

            return Ok(new { message = "Review successully created!" });
        }

        [HttpGet("getAllBookReviews/{bookId}")]
        public async Task<ActionResult<ReviewDto>> GetAllBookReviews(Guid bookId) 
        {
            var reviewDtos = await _service.GetAllBookReviewsAsync(bookId);
            return Ok(reviewDtos);
        }

        [HttpGet("getReviewById/{id}")]
        public async Task<ActionResult<BookDto>> GetReviewById(Guid reviewId)
        {
            var reviewDto = await _service.GetReviewByIdAsync(reviewId);
            if (reviewDto != null)
            {
                return Ok(reviewDto);
            }

            return NotFound(new { message = "Review not found!" });
        }

        [HttpPost("createComment/{readerId}/{reviewId}/{bookId}/{parentCommentId?}")]
        public async Task<IActionResult> CreateComment([FromBody] CommentDto commentDto, Guid readerId, Guid reviewId, Guid bookId, Guid? parentCommentId = null)
        {
            var result = await _service.CreateCommentAsync(commentDto, readerId, reviewId, bookId, parentCommentId);
            if (!result) return BadRequest();

            return Ok(new { message = "Comment created successfully!" });
        }


        [HttpGet("getAllReviewComments/{reviewId}")]
        public async Task<ActionResult<CommentDto>> GetAllReviewComments (Guid reviewId) 
        {
            var commentDtos = await _service.GetAllReviewCommentsAsync(reviewId);
            return Ok(commentDtos);
        }

        [HttpGet("getAllCommentReplies/{parentCommentId}")]
        public async Task<ActionResult<CommentDto>> GetAllCommentReplies(Guid parentCommentId)
        {
            var commentDtos = await _service.GetAllCommentRepliesAsync(parentCommentId);
            return Ok(commentDtos);
        }

        [HttpPost("createReport/{readerId}/{parentId}")]
        public async Task<IActionResult> CreateReport([FromBody]ReportDto reportDto, Guid readerId, Guid parentId) 
        {
            var result = await _service.CreateReportAsync(reportDto, readerId, parentId);

            if (!result)
            {
                return BadRequest();
            }

            return Ok(new { message = "Report successully created!" });
        }

        [HttpGet("getAllReports")]
        public async Task<ActionResult<ReportDto>> GetAllReports() 
        {
            var reportDtos = await _service.GetAllReportsAsync();
            return Ok(reportDtos);
        }

        [HttpPost("addBookToWished/{readerId}/{bookId}")]
        public async Task<IActionResult> AddToWished(Guid readerId, Guid bookId) 
        {
            var result = await _service.AddToWishedAsync(readerId, bookId);
            if (!result) 
            {
                return BadRequest();
            }
            return Ok(new { message = "Book successully added!" });
        }

        [HttpPost("addBookToRead/{readerId}/{bookId}")]
        public async Task<IActionResult> AddToRead(Guid readerId, Guid bookId)
        {
            var result = await _service.AddToReadAsync(readerId, bookId);
            if (!result)
            {
                return BadRequest();
            }
            return Ok(new { message = "Book successully added!" });
        }

        [HttpGet("getAllWished/{readerId}")]
        public async Task<ActionResult<BookDto>> GetAllWished(Guid readerId) 
        {
            var bookDtos = await _service.GetAllWishedAsync(readerId);
            return Ok(bookDtos);
        }

        [HttpGet("getAllRead/{readerId}")]
        public async Task<ActionResult<BookDto>> GetAllRead(Guid readerId)
        {
            var bookDtos = await _service.GetAllReadAsync(readerId);
            return Ok(bookDtos);
        }
    }
}
