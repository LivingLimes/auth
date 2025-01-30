namespace AuthServer.Application.Endpoints.PostToken;

using System.ComponentModel;

public enum PostTokenErrorCode
{
    [Description("invalid_request")]
    InvalidRequest,
    
    [Description("invalid_client")]
    InvalidClient,
    
    [Description("invalid_grant")]
    InvalidGrant,
    
    [Description("unauthorized_client")]
    UnauthorizedClient,
    
    [Description("unsupported_grant_type")]
    UnsupportedGrantType,
    
    [Description("invalid_scope")]
    InvalidScope
}