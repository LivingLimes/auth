namespace AuthServer.Application.Endpoints.PostToken;

using System.Text.Json.Serialization;

public sealed record PostTokenRequest
{
    [JsonPropertyName("grant_type")]
    public required string GrantType { get; init; }

    [JsonPropertyName("code")]
    public required string Code { get; init; }

    [JsonPropertyName("redirect_uri")]
    public string RedirectUri { get; init; } = string.Empty;

    [JsonPropertyName("client_id")]
    public Guid ClientId { get; init; }

    [JsonPropertyName("user_id")]
    public string UserId {get; init; } = string.Empty;
}