using System.Net;
using System.Net.Http.Json;
using Application.DTOs;
using FluentAssertions;
using Xunit;

public class LoansControllerTests : IClassFixture<TestingWebApplicationFactory>
{
    private readonly HttpClient _client;

    public LoansControllerTests(TestingWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateLoan_Then_ReturnBook_ShouldWork()
    {
        // Arrange: loan for seeded BookId 1, MemberId 1
        var createDto = new CreateLoanDto
        {
            BookId = 1,
            MemberId = 1,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        // Create loan
        var createResponse = await _client.PostAsJsonAsync("/api/loans", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdLoan = await createResponse.Content.ReadFromJsonAsync<LoanDto>();
        createdLoan.Should().NotBeNull();

        // Return the book
        var returnResponse = await _client.PostAsync($"/api/loans/{createdLoan!.Id}/return", null);
        returnResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var returnedLoan = await returnResponse.Content.ReadFromJsonAsync<LoanDto>();
        returnedLoan!.ReturnDate.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAllLoans_ShouldReturnOk()
    {
        var response = await _client.GetAsync("/api/loans");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var loans = await response.Content.ReadFromJsonAsync<List<LoanDto>>();
        loans.Should().NotBeNull();
    }
}
