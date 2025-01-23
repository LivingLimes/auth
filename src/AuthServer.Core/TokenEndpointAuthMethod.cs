namespace AuthServer.Core;

using System.ComponentModel;

public enum TokenEndpointAuthMethod
{
    [Description("client_secret_post")]
    ClientSecretPost,
    
    [Description("client_secret_basic")]
    ClientSecretBasic,
    
    [Description("none")]
    None
}