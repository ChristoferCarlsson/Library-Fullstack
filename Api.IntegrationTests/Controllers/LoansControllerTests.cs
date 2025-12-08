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
        var dto = new CreateLoanDto
        {
            BookId = 1,
            MemberId = 1,
            DueDate = DateTime.UtcNow.AddDays(7)
        };

        var createResponse = await _client.PostAsJsonAsync("/api/loans", dto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<LoanDto>();
        created.Should().NotBeNull();

        var returnResponse =
            await _client.PutAsync($"/api/loans/{created!.Id}/return", null);

        returnResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var returned = await returnResponse.Content.ReadFromJsonAsync<LoanDto>();
        returned.Should().NotBeNull();
        returned!.ReturnDate.Should().NotBeNull();
    }
}
