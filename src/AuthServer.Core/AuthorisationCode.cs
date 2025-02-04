namespace AuthServer.Core;

public class AuthorisationCode
{
    public string Code { get; set; }
    public Guid ClientId { get; set; }
    public string RedirectUri { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public string CodeChallenge { get; set; } = string.Empty;
    public string CodeChallengeMethod { get; set; } = string.Empty;
    public string Scopes {get; set; } = string.Empty;
}