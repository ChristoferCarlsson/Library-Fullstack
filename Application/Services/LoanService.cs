using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<LoanService> _logger;

        public LoanService(
            ILoanRepository loanRepository,
            IBookRepository bookRepository,
            IMemberRepository memberRepository,
            IMapper mapper,
            ILogger<LoanService> logger)
        {
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
            _memberRepository = memberRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<List<LoanDto>> GetAllAsync()
        {
            var loans = await _loanRepository.GetAllAsync();
            _logger.LogInformation("Fetched {Count} loans", loans.Count);
            return _mapper.Map<List<LoanDto>>(loans);
        }

        public async Task<LoanDto> GetByIdAsync(int id)
        {
            var loan = await _loanRepository.GetByIdAsync(id);
            if (loan == null)
            {
                _logger.LogWarning("Loan with ID {Id} not found", id);
                throw new NotFoundException($"Loan with id {id} not found.");
            }

            _logger.LogInformation("Fetched loan with ID {Id}", id);
            return _mapper.Map<LoanDto>(loan);
        }

        public async Task<LoanDto> CreateAsync(CreateLoanDto dto)
        {
            _logger.LogInformation("Creating loan for BookId {BookId}, MemberId {MemberId}", dto.BookId, dto.MemberId);

            var book = await _bookRepository.GetByIdAsync(dto.BookId);
            if (book == null)
            {
                _logger.LogWarning("Book with ID {BookId} not found when creating loan", dto.BookId);
                throw new NotFoundException($"Book with id {dto.BookId} not found.");
            }

            var member = await _memberRepository.GetByIdAsync(dto.MemberId);
            if (member == null)
            {
                _logger.LogWarning("Member with ID {MemberId} not found when creating loan", dto.MemberId);
                throw new NotFoundException($"Member with id {dto.MemberId} not found.");
            }

            if (book.CopiesAvailable <= 0)
            {
                _logger.LogWarning("Book with ID {BookId} has no available copies", dto.BookId);
                throw new ValidationException("No copies available for this book.");
            }

            book.CopiesAvailable--;

            var loan = _mapper.Map<Loan>(dto);

            await _loanRepository.AddAsync(loan);
            await _loanRepository.SaveChangesAsync();
            await _bookRepository.SaveChangesAsync();

            _logger.LogInformation("Created loan with ID {Id}", loan.Id);

            return _mapper.Map<LoanDto>(loan);
        }

        public async Task<LoanDto> UpdateAsync(int id, UpdateLoanDto dto)
        {
            var loan = await _loanRepository.GetByIdAsync(id);
            if (loan == null)
            {
                _logger.LogWarning("Attempted update: Loan with ID {Id} not found", id);
                throw new NotFoundException($"Loan with id {id} not found.");
            }

            _mapper.Map(dto, loan);

            _loanRepository.Update(loan);
            await _loanRepository.SaveChangesAsync();

            _logger.LogInformation("Updated loan with ID {Id}", id);

            return _mapper.Map<LoanDto>(loan);
        }

        public async Task DeleteAsync(int id)
        {
            var loan = await _loanRepository.GetByIdAsync(id);
            if (loan == null)
            {
                _logger.LogWarning("Attempted delete: Loan with ID {Id} not found", id);
                throw new NotFoundException($"Loan with id {id} not found.");
            }

            _loanRepository.Remove(loan); // FIXED
            await _loanRepository.SaveChangesAsync();

            _logger.LogWarning("Deleted loan with ID {Id}", id);
        }

        public async Task<LoanDto> ReturnBookAsync(int id)
        {
            _logger.LogInformation("Processing return for LoanId {Id}", id);

            var loan = await _loanRepository.GetByIdAsync(id);
            if (loan == null)
            {
                _logger.LogWarning("Loan with ID {Id} not found for return", id);
                throw new NotFoundException($"Loan with id {id} not found.");
            }

            if (loan.ReturnDate != null)
            {
                _logger.LogWarning("Attempted return: Loan {Id} is already returned", id);
                throw new ValidationException("Book already returned.");
            }

            var book = await _bookRepository.GetByIdAsync(loan.BookId);
            if (book == null) // ✅ THIS is what the compiler wanted
            {
                _logger.LogError(
                    "Invariant violation: Book with ID {BookId} not found for Loan {LoanId}",
                    loan.BookId,
                    id);

                throw new NotFoundException(
                    $"Book with id {loan.BookId} not found for this loan.");
            }

            loan.ReturnDate = DateTime.UtcNow;
            book.CopiesAvailable++;

            _loanRepository.Update(loan);
            await _loanRepository.SaveChangesAsync();
            await _bookRepository.SaveChangesAsync();

            _logger.LogInformation("Book returned for LoanId {Id}", id);

            return _mapper.Map<LoanDto>(loan);
        }
    }
}
