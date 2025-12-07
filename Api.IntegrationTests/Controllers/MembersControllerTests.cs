using System.Net;
using System.Net.Http.Json;
using Application.DTOs;
using FluentAssertions;
using Xunit;

public class MembersControllerTests : IClassFixture<TestingWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MembersControllerTests(TestingWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllMembers_ShouldReturnOk()
    {
        var response = await _client.GetAsync("/api/members");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var members = await response.Content.ReadFromJsonAsync<List<MemberDto>>();
        members.Should().NotBeNull();
        members!.Count.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CreateMember_Then_GetById_ShouldReturnMember()
    {
        var dto = new CreateMemberDto
        {
            FirstName = "Integration",
            LastName = "Member",
            Email = "integration@test.com"
        };

        var createResponse = await _client.PostAsJsonAsync("/api/members", dto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var created = await createResponse.Content.ReadFromJsonAsync<MemberDto>();

        var getResponse = await _client.GetAsync($"/api/members/{created!.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var fetched = await getResponse.Content.ReadFromJsonAsync<MemberDto>();
        fetched!.Email.Should().Be("integration@test.com");
    }
}
