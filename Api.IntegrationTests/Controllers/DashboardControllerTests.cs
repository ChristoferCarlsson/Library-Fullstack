using System.Net;
using System.Net.Http.Json;
using Application.DTOs;
using FluentAssertions;
using Xunit;

public class DashboardControllerTests : IClassFixture<TestingWebApplicationFactory>
{
    private readonly HttpClient _client;

    public DashboardControllerTests(TestingWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDashboard_ShouldReturnStats()
    {
        var response = await _client.GetAsync("/api/dashboard");
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await response.Content.ReadFromJsonAsync<DashboardDto>();
        dto.Should().NotBeNull();

        dto!.TotalBooks.Should().BeGreaterThanOrEqualTo(1);
        dto.TotalMembers.Should().BeGreaterThanOrEqualTo(1);
        dto.ActiveLoans.Should().BeGreaterThanOrEqualTo(0);
        dto.OverdueLoans.Should().BeGreaterThanOrEqualTo(0);
    }
}
