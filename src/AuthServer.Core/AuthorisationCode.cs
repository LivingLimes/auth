namespace AuthServer.Core;

public class AuthorisationCode
{
    public string Code { get; set; }
    public Guid ClientId { get; set; }
    public string RedirectUri { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public string CodeChallenge { get; set; }
    public string CodeChallengeMethod { get; set; } = string.Empty;
}