using System.Net;
using System.Text.Json;
using GardenAssistant.Tests.Infrastructure;
using Shouldly;

namespace GardenAssistant.Tests.Auth;

public class AuthControllerTests(WebAppFixture fixture) : IClassFixture<WebAppFixture>
{
    private readonly HttpClient _client = fixture.CreateClient();

    [Fact]
    public async Task GetToken_WhenSeedUserExists_ShouldReturn200WithTokens()
    {
        fixture.CreateDbContext();

        var response = await _client.GetAsync("/api/auth/token");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body).RootElement;

        json.TryGetProperty("accessToken", out var accessToken).ShouldBeTrue();
        json.TryGetProperty("refreshToken", out var refreshToken).ShouldBeTrue();

        accessToken.GetString().ShouldNotBeNullOrWhiteSpace();
        refreshToken.GetString().ShouldNotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task GetToken_WhenCalledTwice_ShouldReturnDifferentRefreshTokens()
    {
        fixture.CreateDbContext();

        var first = await _client.GetAsync("/api/auth/token");
        var second = await _client.GetAsync("/api/auth/token");

        first.StatusCode.ShouldBe(HttpStatusCode.OK);
        second.StatusCode.ShouldBe(HttpStatusCode.OK);

        var firstJson = JsonDocument.Parse(await first.Content.ReadAsStringAsync()).RootElement;
        var secondJson = JsonDocument.Parse(await second.Content.ReadAsStringAsync()).RootElement;

        firstJson.GetProperty("refreshToken").GetString()
            .ShouldNotBe(secondJson.GetProperty("refreshToken").GetString());
    }
}
