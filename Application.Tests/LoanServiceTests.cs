using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Services;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

public class LoanServiceTests
{
    private readonly Mock<ILoanRepository> _loanRepo;
    private readonly Mock<IBookRepository> _bookRepo;
    private readonly Mock<IMemberRepository> _memberRepo;
    private readonly IMapper _mapper;
    private readonly LoanService _service;

    public LoanServiceTests()
    {
        _loanRepo = new Mock<ILoanRepository>();
        _bookRepo = new Mock<IBookRepository>();
        _memberRepo = new Mock<IMemberRepository>();

        var mapperConfig = new MapperConfiguration(cfg =>
            cfg.AddProfile<Application.Mapping.LibraryProfile>());
        _mapper = mapperConfig.CreateMapper();

        _service = new LoanService(
            _loanRepo.Object,
            _bookRepo.Object,
            _memberRepo.Object,
            _mapper,
            Mock.Of<ILogger<LoanService>>()
        );
    }

    [Fact]
    public async Task Create_ShouldThrow_WhenBookUnavailable()
    {
        _bookRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Domain.Entities.Book { Id = 1, CopiesAvailable = 0 });

        _memberRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Domain.Entities.Member { Id = 1 });

        var dto = new CreateLoanDto { BookId = 1, MemberId = 1 };

        var act = async () => await _service.CreateAsync(dto);

        await act.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task Create_ShouldThrow_WhenMemberMissing()
    {
        _bookRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Domain.Entities.Book { Id = 1, CopiesAvailable = 1 });

        _memberRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Domain.Entities.Member?)null);

        var dto = new CreateLoanDto { BookId = 1, MemberId = 1 };

        var act = async () => await _service.CreateAsync(dto);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task ReturnBook_ShouldThrow_WhenAlreadyReturned()
    {
        _loanRepo.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Domain.Entities.Loan
            {
                Id = 1,
                ReturnDate = DateTime.UtcNow
            });

        var act = async () => await _service.ReturnBookAsync(1);

        await act.Should().ThrowAsync<ValidationException>();
    }
}
