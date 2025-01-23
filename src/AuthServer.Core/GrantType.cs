using System.ComponentModel;

namespace AuthServer.Core;

public enum GrantType
{
    [Description("authorization_code")]
    AuthorizationCode,
    // ClientCredentials,
    // RefreshToken,
    // Password,
    // Implicit,
}