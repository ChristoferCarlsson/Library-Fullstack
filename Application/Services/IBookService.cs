using Application.DTOs;

namespace Application.Interfaces.Services
{
    public interface IBookService
    {
        Task<List<BookDto>> GetAllAsync();
        Task<List<BookDto>> GetByAuthorAsync(int authorId);
        Task<BookDto> GetByIdAsync(int id);
        Task<BookDto> CreateAsync(CreateBookDto dto);
        Task<BookDto> UpdateAsync(int id, UpdateBookDto dto);
        Task DeleteAsync(int id);
        Task<PagedResult<BookDto>> QueryAsync(BookQueryDto query);
    }
}
