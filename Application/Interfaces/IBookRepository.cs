using Domain.Entities;
using Application.DTOs;

namespace Application.Interfaces.Repositories
{
    public interface IBookRepository
    {
        Task<List<Book>> GetAllAsync();
        Task<List<Book>> GetAllByAuthorAsync(int authorId);
        Task<Book?> GetByIdAsync(int id);
        Task AddAsync(Book book);
        void Update(Book book);
        void Remove(Book book);
        Task<bool> SaveChangesAsync();

        Task<PagedResult<Book>> QueryAsync(BookQueryDto query);
    }
}
