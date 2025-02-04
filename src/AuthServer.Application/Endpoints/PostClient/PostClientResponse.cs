namespace AuthServer.Application.Endpoints.PostClient;

using System.Text.Json.Serialization;

public sealed record PostClientResponse
{
    [JsonPropertyName("client_id")]
    public required Guid ClientId { get; init; }

    // [JsonPropertyName("client_secret")]
    // public string ClientSecret { get; init; }

    [JsonPropertyName("client_id_issued_at")]
    public required long ClientIdIssuedAt { get; init; }

    // [JsonPropertyName("client_secret_expires_at")]
    // public long ClientSecretExpiresAt { get; init; }

    [JsonPropertyName("redirect_uris")]
    public required string[] RedirectUris { get; init; }

    [JsonPropertyName("token_endpoint_auth_method")]
    public required string TokenEndpointAuthMethod { get; init; }

    [JsonPropertyName("grant_types")]
    public required string[] GrantTypes { get; init; }

    [JsonPropertyName("response_types")]
    public required string[] ResponseTypes { get; init; }

    [JsonPropertyName("client_name")]
    public required string ClientName { get; init; }

    // [JsonPropertyName("scope")]
    // public required string[] Scope { get; init; }

    // [JsonPropertyName("contacts")]
    // public IEnumerable<string>? Contacts { get; init; }

    [JsonPropertyName("token_lifetime_seconds")]
    public required int AccessTokenLifetimeInSeconds { get; init; }

    [JsonPropertyName("require_pkce")]
    public required bool RequirePkce { get; init; }

    [JsonPropertyName("audience")]
    public required string[] Audience { get; init; }

    [JsonPropertyName("scopes")]
    public required string[] Scopes { get; init; }
}
