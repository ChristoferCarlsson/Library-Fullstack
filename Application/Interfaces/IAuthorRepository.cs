using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IAuthorRepository
    {
        Task<List<Author>> GetAllAsync();
        Task<Author?> GetByIdAsync(int id);
        Task AddAsync(Author author);
        void Update(Author author);
        void Remove(Author author);
        Task<bool> SaveChangesAsync();
        Task<List<Book>> GetBooksByAuthorIdAsync(int authorId);
    }
}
