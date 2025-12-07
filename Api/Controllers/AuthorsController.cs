using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorService _authorService;
        private readonly ILogger<AuthorsController> _logger;

        public AuthorsController(IAuthorService authorService, ILogger<AuthorsController> logger)
        {
            _authorService = authorService;
            _logger = logger;
        }

        // GET: api/authors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAll()
        {
            var authors = await _authorService.GetAllAsync();
            return Ok(authors);
        }

        // GET: api/authors/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AuthorDto>> GetById(int id)
        {
            var author = await _authorService.GetByIdAsync(id);
            return Ok(author); // NotFound handled by middleware
        }

        // POST: api/authors
        [HttpPost]
        public async Task<ActionResult<AuthorDto>> Create([FromBody] CreateAuthorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _authorService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/authors/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<AuthorDto>> Update(int id, [FromBody] UpdateAuthorDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _authorService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        // DELETE: api/authors/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _authorService.DeleteAsync(id);
            return NoContent();
        }
    }
}
