namespace AuthServer.Application.Endpoints.PostToken;

using System.Text.Json.Serialization;

public sealed record PostTokenResponse
{
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; init; }

    [JsonPropertyName("token_type")]
    public required string TokenType { get; init; }

    [JsonPropertyName("expires_in")]
    public required int ExpiresIn { get; init; }

    // [JsonPropertyName("refresh_token")]
    // public required string RefreshToken { get; init; }

    // [JsonPropertyName("scope")]
    // public required string Scope { get; init; }
}