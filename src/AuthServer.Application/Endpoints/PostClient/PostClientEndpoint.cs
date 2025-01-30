using AuthServer.Application.Endpoints.PostRegisterClient;
using AuthServer.Core;
using AuthServer.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Application.Endpoints.PostClient;

public class PostClientEndpoint
{
    public static async Task<IResult> HandleAsync(
        PostClientRequest request,
        AuthDbContext dbContext,
        ILogger<PostClientEndpoint> logger)
    {
        logger.LogInformation("Received client registration request for client name: {ClientName}", request.ClientName);

        var validationResult = ValidateRequest(request);
        if (validationResult != null)
        {
            return validationResult;
        }

        var clients = await dbContext.Clients.Where(c => c.Name == request.ClientName).ToListAsync();
        if (clients.Count > 0)
        {
            return Results.Conflict(new PostClientError
            {
                Error = PostClientErrorCode.InvalidClientMetadata.GetDescription(),
                ErrorDescription = "Client name must be unique."
            });
        }

        var grantTypes = AllowedGrantTypes.Create(request.AllowedGrantTypes);
        var redirectUris = RedirectUris.Create(request.RedirectUris);
        var allowedResponseTypes = AllowedResponseTypes.Create(request.AllowedResponseTypes);

        var client = Client.Create(
            name: request.ClientName,
            allowedGrantTypes: grantTypes,
            redirectUris: redirectUris,
            tokenEndpointAuthMethod: EnumMethods.ParseFromDescription<TokenEndpointAuthMethod>(request.TokenEndpointAuthMethod),
            allowedResponseTypes: allowedResponseTypes,
            requirePkce: request.RequirePkce,
            accessTokenLifetimeInSeconds: request.AccessTokenLifetimeInSeconds
        );

        await dbContext.Clients.AddAsync(client);
        await dbContext.SaveChangesAsync();

        logger.LogInformation("Client registration completed for client id: {ClientId}", client.Id);

        // We return all the fields stored in the client registration so that the client application knows what was created.
        // It is possible that values were modified during registration and defaults may have been applied to some fields not sent by the client application.
        var response = new PostClientResponse
        {
            ClientId = client.Id,
            ClientIdIssuedAt = client.CreatedOn.Ticks,
            ClientName = client.Name,
            GrantTypes = client.AllowedGrantTypes.Value.Select(gt => gt.GrantType.GetDescription()).ToArray(),
            RedirectUris = client.RedirectUris.Value.Select(ru => ru.Value).ToArray(),
            TokenEndpointAuthMethod = client.TokenEndpointAuthMethod.GetDescription(),
            ResponseTypes = client.AllowedResponseTypes.Value.Select(rt => rt.ResponseType.GetDescription()).ToArray(),
            RequirePkce = client.RequirePkce,
            AccessTokenLifetimeInSeconds = client.AccessTokenLifetimeInSeconds
        };

        return Results.Created($"/register", response);
    }

    private static IResult? ValidateRequest(PostClientRequest request)
    {
        if ((request.AllowedGrantTypes.Contains("implicit") || request.AllowedGrantTypes.Contains("authorization_code")) && request.RedirectUris.Length == 0)
        {
            return Results.BadRequest(new PostClientError
            {
                Error = PostClientErrorCode.InvalidClientMetadata.GetDescription(),
                ErrorDescription = "Implicit grant type and authorisation code grant type require at least one redirect uri"
            });
        }

        if (!AllowedGrantTypes.CanCreate(request.AllowedGrantTypes))
        {
            return Results.BadRequest(new PostClientError
            {
                // Add configuration endpoint to return valid grant types.
                Error = PostClientErrorCode.InvalidClientMetadata.GetDescription(),
                ErrorDescription = "Invalid grant types specified."
            });
        }

        if (!RedirectUris.CanCreate(request.RedirectUris))
        {
            return Results.BadRequest(new PostClientError
            {
                Error = PostClientErrorCode.InvalidRedirectUri.GetDescription(),
                ErrorDescription = "Invalid redirect URIs specified. A redirect uri must be an absolute uri, HTTPS scheme unless it is localhost, contain no fragments and not contain any wildcards."
            });
        }

        if (request.AccessTokenLifetimeInSeconds < 0)
        {
            return Results.BadRequest(new PostClientError
            {
                Error = PostClientErrorCode.InvalidClientMetadata.GetDescription(),
                ErrorDescription = "Access token lifetime must be greater than 0."
            });
        }

        if (request.ClientName.Length > 100)
        {
            return Results.BadRequest(new PostClientError
            {
                Error = PostClientErrorCode.InvalidClientMetadata.GetDescription(),
                ErrorDescription = "Client name must be less than 100 characters."
            });
        }

        if (TokenEndpointAuthMethod.None.GetDescription() != request.TokenEndpointAuthMethod &&
            TokenEndpointAuthMethod.ClientSecretPost.GetDescription() != request.TokenEndpointAuthMethod &&
            TokenEndpointAuthMethod.ClientSecretBasic.GetDescription() != request.TokenEndpointAuthMethod)
        {
            return Results.BadRequest(new PostClientError
            {
                Error = PostClientErrorCode.InvalidClientMetadata.GetDescription(),
                ErrorDescription = "Invalid token endpoint auth method specified."
            });
        }

        return null;
    }
}