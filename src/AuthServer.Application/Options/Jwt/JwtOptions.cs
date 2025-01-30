namespace AuthServer.Application.Options.Jwt;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    public string AccessTokenSigningAlgorithm { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
}