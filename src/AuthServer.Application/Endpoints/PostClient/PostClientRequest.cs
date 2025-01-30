namespace AuthServer.Application.Endpoints.PostClient;

using System.Text.Json.Serialization;

// Default values are specified in the spec: https://www.rfc-editor.org/rfc/rfc7591#section-2
public sealed record PostClientRequest
{
    [JsonPropertyName("redirect_uris")]
    public string[] RedirectUris { get; init; } = Array.Empty<string>();

    [JsonPropertyName("token_endpoint_auth_method")]
    public string TokenEndpointAuthMethod { get; init; } = "client_secret_post";

    [JsonPropertyName("grant_types")]
    public string[] AllowedGrantTypes { get; init; } = ["authorization_code"];

    [JsonPropertyName("response_types")]
    public string[] AllowedResponseTypes { get; init; } = ["code"];

    [JsonPropertyName("client_name")]
    public string ClientName { get; init; } = string.Empty;

    [JsonPropertyName("scope")]
    public string Scopes { get; init; } = string.Empty;

    // [JsonPropertyName("contacts")]
    // public string[]? Contacts { get; init; }

    // Custom properties
    [JsonPropertyName("access_token_lifetime_seconds")]
    public int AccessTokenLifetimeInSeconds { get; init; }

    [JsonPropertyName("audience")]
    public string[] Audience { get; init; } = Array.Empty<string>();

    [JsonPropertyName("require_pkce")]
    public bool RequirePkce { get; init; }

    [JsonPropertyName("tenant_id")]
    public Guid TenantId { get; init; }
}
