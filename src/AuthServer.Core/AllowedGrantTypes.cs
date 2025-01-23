namespace AuthServer.Core;

public class AllowedGrantTypes
{
    private static HashSet<string> validGrantTypeSet = new HashSet<string>
    {
        "authorization_code",
        // "client_credentials",
        // "refresh_token",
        // "password",
        // "implicit",
        // "urn:ietf:params:oauth:grant-type:jwt-bearer",
        // "urn:ietf:params:oauth:grant-type:saml2-bearer",
    };

    public AllowedGrantType[] Value { get; private set; } = Array.Empty<AllowedGrantType>();

    private AllowedGrantTypes()
    {
    }

    private AllowedGrantTypes(string[] grantTypes)
    {
        Value = grantTypes.Select(grantType => AllowedGrantType.Create(EnumMethods.ParseFromDescription<GrantType>(grantType))).ToArray();
    }

    public static bool CanCreate(string[] grantTypes)
    {
        if (!grantTypes.Any())
        {
            return false;
        }
        if (!grantTypes.All(validGrantTypeSet.Contains))
        {
            return false;
        }
        return true;
    }

    public static AllowedGrantTypes Create(string[] grantTypes)
    {
        if (!CanCreate(grantTypes))
        {
            throw new Exception($"Supplied grant types: '{string.Join(", ", grantTypes)}' are invalid.");
        }
        return new AllowedGrantTypes(grantTypes);
    }
}
