using System.Net;
using System.Net.Http.Json;
using Application.DTOs;
using FluentAssertions;
using Xunit;

public class AuthorsControllerTests : IClassFixture<TestingWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthorsControllerTests(TestingWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ShouldReturnOk_WithList()
    {
        var response = await _client.GetAsync("/api/authors");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var authors = await response.Content.ReadFromJsonAsync<List<AuthorDto>>();
        authors.Should().NotBeNull();
        authors!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Create_Then_GetById_ShouldReturnAuthor()
    {
        var dto = new CreateAuthorDto
        {
            FirstName = "Integration",
            LastName = "Author",
            Description = "Created in tests"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/authors", dto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<AuthorDto>();
        created.Should().NotBeNull();

        var getResponse = await _client.GetAsync($"/api/authors/{created!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetched = await getResponse.Content.ReadFromJsonAsync<AuthorDto>();
        fetched!.FullName.Should().Be("Integration Author");
    }
}
