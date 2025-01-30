namespace AuthServer.Application.Endpoints.PostToken;

public sealed record PostTokenError
{
    public required string Error { get; init; } = string.Empty;
    public required string ErrorDescription { get; init; } = string.Empty;
    public string ErrorUri { get; init; } = string.Empty;
}


