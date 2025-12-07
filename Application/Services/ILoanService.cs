using Application.DTOs;

namespace Application.Interfaces.Services
{
    public interface ILoanService
    {
        Task<List<LoanDto>> GetAllAsync();
        Task<LoanDto> GetByIdAsync(int id);
        Task<LoanDto> CreateAsync(CreateLoanDto dto);
        Task<LoanDto> UpdateAsync(int id, UpdateLoanDto dto);

        // Optional delete
        Task DeleteAsync(int id);

        // Optional advanced: return loan
        Task<LoanDto> ReturnBookAsync(int id);
    }
}
