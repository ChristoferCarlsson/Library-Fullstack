using Application.DTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly ILogger<DashboardService> _logger;

        public DashboardService(
            IBookRepository bookRepository,
            IMemberRepository memberRepository,
            ILoanRepository loanRepository,
            ILogger<DashboardService> logger)
        {
            _bookRepository = bookRepository;
            _memberRepository = memberRepository;
            _loanRepository = loanRepository;
            _logger = logger;
        }

        public async Task<DashboardDto> GetDashboardAsync()
        {
            _logger.LogInformation("Fetching dashboard statistics...");

            var books = await _bookRepository.GetAllAsync();
            var members = await _memberRepository.GetAllAsync();
            var loans = await _loanRepository.GetAllAsync();

            int activeLoans = loans.Count(l => l.ReturnDate == null);
            int overdueLoans = loans.Count(l =>
                l.DueDate < DateTime.UtcNow && l.ReturnDate == null
            );

            _logger.LogInformation(
                "Dashboard stats computed: {Books} books, {Members} members, {Active} active loans, {Overdue} overdue loans",
                books.Count,
                members.Count,
                activeLoans,
                overdueLoans
            );

            return new DashboardDto
            {
                TotalBooks = books.Count,
                TotalMembers = members.Count,
                ActiveLoans = activeLoans,
                OverdueLoans = overdueLoans
            };
        }
    }
}
