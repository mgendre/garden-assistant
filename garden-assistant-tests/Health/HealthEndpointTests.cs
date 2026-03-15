using System.Net;
using System.Text.Json;
using GardenAssistant.Tests.Infrastructure;
using Shouldly;

namespace GardenAssistant.Tests.Health;

public class HealthEndpointTests(WebAppFixture fixture) : IClassFixture<WebAppFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();

    [Fact]
    public async Task GetHealth_WhenCalled_ShouldReturn200WithHealthyStatus()
    {
        var response = await _client.GetAsync("/health");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body).RootElement;

        json.TryGetProperty("status", out var status).ShouldBeTrue();
        status.GetString().ShouldBe("healthy");
    }
}
