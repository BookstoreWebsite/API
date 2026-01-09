using Bookstore.Application.DTO;
using Bookstore.Application.IService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IGenreService _service;
        public GenreController(IGenreService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<GenreDto>>> GetAll()
        {
            var genreDtos = await _service.GetAllAsync();
            return Ok(genreDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GenreDto>> GetById(Guid id)
        {
            var genreDto = await _service.GetByIdAsync(id);
            if (genreDto != null)
            {
                return Ok(genreDto);
            }

            return NotFound(new { message = "Genre not found!" });
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody] GenreFormsDto genreFormsDto)
        {
            var result = await _service.CreateAsync(genreFormsDto);

            if (!result)
            {
                return BadRequest();
            }
            return Ok(new { message = "Genre successfully created!" });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] GenreFormsDto genreFormsDto)
        {
            var result = await _service.UpdateAsync(id, genreFormsDto);

            if (!result)
            {
                return NotFound(new { message = "Genre not found!" });
            }

            return Ok(new { message = "Genre successully updated!" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _service.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new { message = "Genre not found" });
            }

            return Ok(new { message = "Genre successfully deleted!" });
        }
    }
}
