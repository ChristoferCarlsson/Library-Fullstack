using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;

namespace Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly ILoanRepository _loanRepository;

        public DashboardService(
            IBookRepository bookRepository,
            IMemberRepository memberRepository,
            ILoanRepository loanRepository)
        {
            _bookRepository = bookRepository;
            _memberRepository = memberRepository;
            _loanRepository = loanRepository;
        }

        public async Task<DashboardDto> GetDashboardAsync()
        {
            var books = await _bookRepository.GetAllAsync();
            var members = await _memberRepository.GetAllAsync();
            var loans = await _loanRepository.GetAllAsync();

            return new DashboardDto
            {
                TotalBooks = books.Count,
                TotalMembers = members.Count,
                ActiveLoans = loans.Count(l => l.ReturnDate == null),
                OverdueLoans = loans.Count(l => l.DueDate < DateTime.UtcNow && l.ReturnDate == null)
            };
        }
    }
}
