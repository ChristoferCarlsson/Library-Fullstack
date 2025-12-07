using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using AutoMapper;
using Domain.Entities;

namespace Application.Services
{
    public class LoanService : ILoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IMapper _mapper;

        public LoanService(
            ILoanRepository loanRepository,
            IBookRepository bookRepository,
            IMemberRepository memberRepository,
            IMapper mapper)
        {
            _loanRepository = loanRepository;
            _bookRepository = bookRepository;
            _memberRepository = memberRepository;
            _mapper = mapper;
        }

        public async Task<List<LoanDto>> GetAllAsync()
        {
            var loans = await _loanRepository.GetAllAsync();
            return _mapper.Map<List<LoanDto>>(loans);
        }

        public async Task<LoanDto> GetByIdAsync(int id)
        {
            var loan = await _loanRepository.GetByIdAsync(id);
            if (loan == null)
                throw new NotFoundException($"Loan with id {id} not found.");

            return _mapper.Map<LoanDto>(loan);
        }

        public async Task<LoanDto> CreateAsync(CreateLoanDto dto)
        {
            var book = await _bookRepository.GetByIdAsync(dto.BookId);
            if (book == null)
                throw new NotFoundException($"Book with id {dto.BookId} not found.");

            var member = await _memberRepository.GetByIdAsync(dto.MemberId);
            if (member == null)
                throw new NotFoundException($"Member with id {dto.MemberId} not found.");

            if (book.CopiesAvailable <= 0)
                throw new ValidationException("No copies available for this book.");

            book.CopiesAvailable--;

            var loan = _mapper.Map<Loan>(dto);

            await _loanRepository.AddAsync(loan);
            await _loanRepository.SaveChangesAsync();
            await _bookRepository.SaveChangesAsync();

            return _mapper.Map<LoanDto>(loan);
        }

        public async Task<LoanDto> UpdateAsync(int id, UpdateLoanDto dto)
        {
            var loan = await _loanRepository.GetByIdAsync(id);
            if (loan == null)
                throw new NotFoundException($"Loan with id {id} not found.");

            _mapper.Map(dto, loan);

            _loanRepository.Update(loan);
            await _loanRepository.SaveChangesAsync();

            return _mapper.Map<LoanDto>(loan);
        }

        public async Task DeleteAsync(int id)
        {
            var loan = await _loanRepository.GetByIdAsync(id);
            if (loan == null)
                throw new NotFoundException($"Loan with id {id} not found.");

            _loanRepository.Update(loan);
            await _loanRepository.SaveChangesAsync();
        }

        public async Task<LoanDto> ReturnBookAsync(int id)
        {
            var loan = await _loanRepository.GetByIdAsync(id);
            if (loan == null)
                throw new NotFoundException($"Loan with id {id} not found.");

            if (loan.ReturnDate != null)
                throw new ValidationException("Book already returned.");

            loan.ReturnDate = DateTime.UtcNow;

            var book = await _bookRepository.GetByIdAsync(loan.BookId);
            book.CopiesAvailable++;

            _loanRepository.Update(loan);
            await _loanRepository.SaveChangesAsync();
            await _bookRepository.SaveChangesAsync();

            return _mapper.Map<LoanDto>(loan);
        }
    }
}
