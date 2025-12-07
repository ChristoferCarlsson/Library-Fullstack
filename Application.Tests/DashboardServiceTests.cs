using Application.Services;
using Application.Interfaces.Repositories;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging;

public class DashboardServiceTests
{
    private readonly Mock<IBookRepository> _books;
    private readonly Mock<IMemberRepository> _members;
    private readonly Mock<ILoanRepository> _loans;
    private readonly Mock<ILogger<DashboardService>> _logger;
    private readonly DashboardService _service;

    public DashboardServiceTests()
    {
        _books = new Mock<IBookRepository>();
        _members = new Mock<IMemberRepository>();
        _loans = new Mock<ILoanRepository>();
        _logger = new Mock<ILogger<DashboardService>>();

        _service = new DashboardService(
            _books.Object,
            _members.Object,
            _loans.Object,
            _logger.Object
        );
    }

    [Fact]
    public async Task Dashboard_ShouldCalculateCountsCorrectly()
    {
        _books.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Domain.Entities.Book> { new() });
        _members.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Domain.Entities.Member> { new() });
        _loans.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Domain.Entities.Loan>());

        var result = await _service.GetDashboardAsync();

        result.TotalBooks.Should().Be(1);
        result.TotalMembers.Should().Be(1);
        result.ActiveLoans.Should().Be(0);
        result.OverdueLoans.Should().Be(0);
    }
}
