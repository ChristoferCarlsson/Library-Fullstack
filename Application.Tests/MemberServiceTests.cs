using Application.DTOs;
using Application.Exceptions;
using Application.Interfaces.Repositories;
using Application.Services;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class MemberServiceTests
{
    private readonly Mock<IMemberRepository> _memberRepoMock;
    private readonly Mock<IBookRepository> _bookRepoMock;
    private readonly Mock<ILoanRepository> _loanRepoMock;
    private readonly IMapper _mapper;
    private readonly MemberService _service;

    public MemberServiceTests()
    {
        _memberRepoMock = new Mock<IMemberRepository>();
        _bookRepoMock = new Mock<IBookRepository>();
        _loanRepoMock = new Mock<ILoanRepository>();

        var mapperConfig = new MapperConfiguration(cfg =>
            cfg.AddProfile<Application.Mapping.LibraryProfile>());

        _mapper = mapperConfig.CreateMapper();

        _service = new MemberService(
            _memberRepoMock.Object,
            _bookRepoMock.Object,
            _loanRepoMock.Object,
            _mapper,
            Mock.Of<ILogger<MemberService>>()
        );
    }

    [Fact]
    public async Task GetById_ShouldThrow_WhenMissing()
    {
        _memberRepoMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Domain.Entities.Member?)null);

        var act = async () => await _service.GetByIdAsync(1);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    [Fact]
    public async Task Delete_ShouldRemoveMember_And_ReturnBooks()
    {
        // Arrange
        var member = new Domain.Entities.Member
        {
            Id = 1,
            Loans = new List<Domain.Entities.Loan>
            {
                new()
                {
                    Id = 10,
                    BookId = 5,
                    ReturnDate = null
                }
            }
        };

        var book = new Domain.Entities.Book
        {
            Id = 5,
            CopiesAvailable = 2
        };

        _memberRepoMock
            .Setup(r => r.GetByIdWithLoansAsync(1))
            .ReturnsAsync(member);

        _bookRepoMock
            .Setup(r => r.GetByIdAsync(5))
            .ReturnsAsync(book);

        // Act
        await _service.DeleteAsync(1);

        // Assert — inventory restored
        book.CopiesAvailable.Should().Be(3);

        // Loan removed
        _loanRepoMock.Verify(
            r => r.Remove(It.IsAny<Domain.Entities.Loan>()),
            Times.Once
        );

        // Member removed
        _memberRepoMock.Verify(
            r => r.Remove(member),
            Times.Once
        );

        // Saves executed
        _loanRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        _bookRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        _memberRepoMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
