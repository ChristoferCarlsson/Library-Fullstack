using Application.DTOs;

namespace Application.Interfaces.Services
{
    public interface IAuthorService
    {
        Task<List<AuthorDto>> GetAllAsync();
        Task<AuthorDto> GetByIdAsync(int id);
        Task<AuthorDto> CreateAsync(CreateAuthorDto dto);
        Task<AuthorDto> UpdateAsync(int id, UpdateAuthorDto dto);
        Task DeleteAsync(int id);
    }
}