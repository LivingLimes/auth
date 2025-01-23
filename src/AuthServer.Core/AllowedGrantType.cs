namespace AuthServer.Core;

public class AllowedGrantType
{
    public Guid ClientId { get; init; }
    public GrantType GrantType { get; set; }

    private AllowedGrantType() { }

    private AllowedGrantType(GrantType grantType)
    {
        GrantType = grantType;
    }

    public static AllowedGrantType Create(GrantType grantType)
    {
        return new AllowedGrantType(grantType);
    }
}