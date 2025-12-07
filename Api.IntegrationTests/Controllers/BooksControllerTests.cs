using System.Net;
using System.Net.Http.Json;
using Application.DTOs;
using FluentAssertions;
using Xunit;

public class BooksControllerTests : IClassFixture<TestingWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BooksControllerTests(TestingWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllBooks_ShouldReturnOk_WithList()
    {
        var response = await _client.GetAsync("/api/books");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var books = await response.Content.ReadFromJsonAsync<List<BookDto>>();
        books.Should().NotBeNull();
        books!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CreateBook_WithExistingAuthor_ShouldSucceed()
    {
        var dto = new CreateBookDto
        {
            Title = "Integration Book",
            AuthorId = 1,
            ISBN = "INT-BOOK-001",
            PublishedDate = DateTime.UtcNow,
            CopiesTotal = 2
        };

        var response = await _client.PostAsJsonAsync("/api/books", dto);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await response.Content.ReadFromJsonAsync<BookDto>();
        created.Should().NotBeNull();
        created!.Title.Should().Be("Integration Book");
    }
}
