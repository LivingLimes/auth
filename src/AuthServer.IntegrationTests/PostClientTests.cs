using System.Net.Http.Json;
using AuthServer.Application.Endpoints.PostClient;
using AuthServer.Core;
using AuthServer.Infrastructure.Data;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AuthServer.IntegrationTests;

public class PostClientTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public PostClientTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
        });
    }

    [Fact]
    public async Task PostClient_CreatesClientSuccessfully()
    {
        var client = _factory.CreateClient();
        var request = new PostClientRequest
        {
            ClientName = "TestClient",
            AllowedGrantTypes = ["authorization_code"],
            RedirectUris = ["https://localhost/callback"],
            TokenEndpointAuthMethod = TokenEndpointAuthMethod.ClientSecretPost.GetDescription(),
            AllowedResponseTypes = ["code"],
            AccessTokenLifetimeInSeconds = 3600,
            RequirePkce = false,
            Audience = ["https://localhost/callback", "other audience"],
            Scopes = "scope1 scope2",
        };

        var response = await client.PostAsJsonAsync("/client", request);

        Console.WriteLine(response.Content.ReadAsStringAsync().Result);
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadFromJsonAsync<PostClientResponse>();
        
        responseData?.ClientId.Should().NotBeEmpty();
        responseData?.ClientName.Should().Be(request.ClientName);
        responseData?.GrantTypes.Should().BeEquivalentTo(request.AllowedGrantTypes);
        responseData?.RedirectUris.Should().BeEquivalentTo(request.RedirectUris);
        responseData?.TokenEndpointAuthMethod.Should().Be(request.TokenEndpointAuthMethod);
        responseData?.ResponseTypes.Should().BeEquivalentTo(request.AllowedResponseTypes);
        responseData?.RequirePkce.Should().Be(request.RequirePkce);
        responseData?.AccessTokenLifetimeInSeconds.Should().Be(request.AccessTokenLifetimeInSeconds);
        responseData?.Audience.Should().BeEquivalentTo(request.Audience);
        responseData?.Scopes.Should().BeEquivalentTo(request.Scopes.Split(' '));
    }
} 
