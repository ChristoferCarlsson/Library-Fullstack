using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBookService _bookService;
        private readonly ILogger<BooksController> _logger;

        public BooksController(IBookService bookService, ILogger<BooksController> logger)
        {
            _bookService = bookService;
            _logger = logger;
        }

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetAll()
        {
            var books = await _bookService.GetAllAsync();
            return Ok(books);
        }

        // GET: api/books/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookDto>> GetById(int id)
        {
            var book = await _bookService.GetByIdAsync(id);
            return Ok(book); // NotFound is handled by middleware
        }

        // GET: api/books/by-author/{authorId}
        [HttpGet("by-author/{authorId:int}")]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetByAuthor(int authorId)
        {
            var books = await _bookService.GetByAuthorAsync(authorId);
            return Ok(books);
        }

        // GET: api/books/search
        // Example: api/books/search?Title=Harry&Page=1&PageSize=10&SortBy=Title&Desc=false
        [HttpGet("search")]
        public async Task<ActionResult<PagedResult<BookDto>>> Search([FromQuery] BookQueryDto query)
        {
            var result = await _bookService.QueryAsync(query);
            return Ok(result);
        }

        // POST: api/books
        [HttpPost]
        public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _bookService.CreateAsync(dto);

            // returns 201 with Location header
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/books/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<BookDto>> Update(int id, [FromBody] UpdateBookDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _bookService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        // DELETE: api/books/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _bookService.DeleteAsync(id);
            return NoContent();
        }
    }
}
