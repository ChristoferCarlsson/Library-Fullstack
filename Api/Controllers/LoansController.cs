using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoansController : ControllerBase
    {
        private readonly ILoanService _loanService;
        private readonly ILogger<LoansController> _logger;

        public LoansController(
            ILoanService loanService,
            ILogger<LoansController> logger)
        {
            _loanService = loanService;
            _logger = logger;
        }

        // GET: api/loans
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LoanDto>>> GetAll()
        {
            var loans = await _loanService.GetAllAsync();
            return Ok(loans);
        }

        // GET: api/loans/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<LoanDto>> GetById(int id)
        {
            var loan = await _loanService.GetByIdAsync(id);
            return Ok(loan); // NotFound handled by middleware
        }

        // POST: api/loans
        [HttpPost]
        public async Task<ActionResult<LoanDto>> Create([FromBody] CreateLoanDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var created = await _loanService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }

        // PUT: api/loans/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<LoanDto>> Update(
            int id,
            [FromBody] UpdateLoanDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updated = await _loanService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        // PUT: api/loans/{id}/return
        [HttpPut("{id:int}/return")]
        public async Task<ActionResult<LoanDto>> ReturnBook(int id)
        {
            var returnedLoan = await _loanService.ReturnBookAsync(id);
            return Ok(returnedLoan);
        }

        // DELETE: api/loans/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _loanService.DeleteAsync(id);
            return NoContent();
        }
    }
}
