using Bookstore.Application.DTO;
using Bookstore.Application.IService;
using Bookstore.Domain.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IBookService _bookService;
        public BookController(IBookService bookService)
        {
            _bookService = bookService;
        }

        [HttpGet]
        public async Task<ActionResult<List<BookDto>>> GetAll()
        {
            var bookDtos = await _bookService.GetAllAsync();
            return Ok(bookDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookDto>> GetById(Guid id) 
        {
            var bookDto = await _bookService.GetByIdAsync(id);
            if(bookDto != null) 
            {
                return Ok(bookDto);
            }

            return NotFound(new {message = "Book not found!" });
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody]BookDto bookDto) 
        {
            var result = await _bookService.CreateAsync(bookDto);

            if (!result) 
            {
                return BadRequest();
            }
            return Ok(new { message = "Book successfully created!" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody]BookDto bookDto) 
        {
            var result = await _bookService.UpdateAsync(id, bookDto);

            if (!result) 
            {
                return NotFound(new { message = "Book not found!" });
            }

            return Ok(new {message = "Book successully updated!" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id) 
        {
            var result = await _bookService.DeleteAsync(id);

            if (!result) 
            {
                return NotFound(new { message = "Book not found" });
            }

            return Ok(new { message = "Book successfully deleted!" });
        }
    }
}
