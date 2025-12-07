using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ILoanRepository
    {
        Task<List<Loan>> GetAllAsync();
        Task<Loan?> GetByIdAsync(int id);
        Task AddAsync(Loan loan);
        void Update(Loan loan);
        Task<bool> SaveChangesAsync();
        void Remove(Loan loan);
    }
}
