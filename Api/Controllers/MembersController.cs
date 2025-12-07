using Application.DTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _memberService;
        private readonly ILogger<MembersController> _logger;

        public MembersController(IMemberService memberService, ILogger<MembersController> logger)
        {
            _memberService = memberService;
            _logger = logger;
        }

        // GET: api/members
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetAll()
        {
            var members = await _memberService.GetAllAsync();
            return Ok(members);
        }

        // GET: api/members/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<MemberDto>> GetById(int id)
        {
            var member = await _memberService.GetByIdAsync(id);
            return Ok(member); // NotFound handled by middleware
        }

        // POST: api/members
        [HttpPost]
        public async Task<ActionResult<MemberDto>> Create([FromBody] CreateMemberDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _memberService.CreateAsync(dto);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/members/{id}
        [HttpPut("{id:int}")]
        public async Task<ActionResult<MemberDto>> Update(int id, [FromBody] UpdateMemberDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _memberService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        // DELETE: api/members/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _memberService.DeleteAsync(id);
            return NoContent();
        }
    }
}
