using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IMemberRepository
    {
        Task<List<Member>> GetAllAsync();
        Task<Member?> GetByIdAsync(int id);
        Task AddAsync(Member member);
        void Update(Member member);
        void Remove(Member member);
        Task<bool> SaveChangesAsync();
        Task<Member?> GetByIdWithLoansAsync(int id);

    }
}
