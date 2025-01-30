namespace AuthServer.Application.Endpoints.PostRegisterClient;

public sealed record PostClientError
{
    public required string Error { get; init; }
    public string ErrorDescription { get; init; } = string.Empty;
}
