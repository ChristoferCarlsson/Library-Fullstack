using Application.DTOs;

namespace Application.Interfaces.Services
{
    public interface IMemberService
    {
        Task<List<MemberDto>> GetAllAsync();
        Task<MemberDto> GetByIdAsync(int id);
        Task<MemberDto> CreateAsync(CreateMemberDto dto);
        Task<MemberDto> UpdateAsync(int id, UpdateMemberDto dto);
        Task DeleteAsync(int id);
    }
}
