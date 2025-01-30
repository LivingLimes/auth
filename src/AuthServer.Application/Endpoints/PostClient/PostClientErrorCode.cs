namespace AuthServer.Application.Endpoints.PostRegisterClient;

using System.ComponentModel;

// Standardised values are in the spec: https://www.rfc-editor.org/rfc/rfc7591#section-3.2.2
public enum PostClientErrorCode
{
    [Description("invalid_redirect_uri")]
    InvalidRedirectUri,

    [Description("invalid_client_metadata")]
    InvalidClientMetadata,

    // [Description("invalid_software_statement")]
    // InvalidSoftwareStatement,

    // [Description("unapproved_software_statement")]
    // UnapprovedSoftwareStatement
}