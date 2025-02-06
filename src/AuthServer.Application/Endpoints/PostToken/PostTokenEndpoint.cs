namespace AuthServer.Application.Endpoints.PostToken;

using System.Text.Json;
using AuthServer.Application.Constants;
using AuthServer.Application.Options.Jwt;
using AuthServer.Core;
using AuthServer.Core.Interface;
using AuthServer.Infrastructure.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

public class PostTokenEndpoint
{
    public static async Task<IResult> HandleAsync(
        PostTokenRequest request,
        IMemoryCache memoryCache,
        AuthDbContext dbContext,
        IJwtGenerator tokenGenerator,
        HttpContext httpContext,
        IOptions<JwtOptions> jwtOptions,
        ILogger<PostTokenEndpoint> logger)
    {
        // Duplicate properties in the request don't need to be handled for a json payload because json cannot have duplicate properties.

        logger.LogInformation("Handling token request for client {ClientId}", request.ClientId);

        if (request.GrantType == "refresh_token")
        {
            // Refresh token must exist
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return Results.BadRequest(new PostTokenError
                {
                    Error = PostTokenErrorCode.InvalidRequest.GetDescription(),
                    ErrorDescription = "Refresh token is required"
                });
            }

            // Rotate the refresh token
            var refreshTokenKey = $"{CacheKeys.RefreshTokenCodePrefix}{request.RefreshToken}";
            var rt = memoryCache.Get(refreshTokenKey);

            if (rt == null)
            {
                return Results.BadRequest(new PostTokenError
                {
                    Error = PostTokenErrorCode.InvalidRequest.GetDescription(),
                    ErrorDescription = "Invalid refresh token"
                });
            }

            // if () Try validating signature

            

            memoryCache.Remove(refreshTokenKey);

            // var now = DateTime.UtcNow;
            // var newAccessTokenPayload = new Dictionary<string, object>
            // { 
            //     { JwtClaims.Subject, request.UserId },
            //     { JwtClaims.Issuer, jwtOptions.Value.Issuer },
            //     // { JwtClaims.Audience, new string[] { } },
            //     { JwtClaims.Expiration, now.AddSeconds(client.AccessTokenLifetimeInSeconds).ToString() },
            //     { JwtClaims.IssuedAt, now.ToString() },
            //     { JwtClaims.JwtId, Guid.NewGuid().ToString() },
            //     { JwtClaims.NotBefore, now.ToString() }
            // };
            // var newAccessToken = tokenGenerator.GenerateJwt(algo: jwtOptions.Value.AccessTokenSigningAlgorithm, payload: new Dictionary<string, string> { { "sub", "123" } });
            // var newRefreshToken = tokenGenerator.GenerateJwt(algo: jwtOptions.Value.RefreshTokenSigningAlgorithm, new Dictionary<string, string> { { "", ""} });

            // var newRefreshTokenObject = new RefreshToken
            // {
            //     // TODO: Client id
            //     Token = newRefreshToken  
            // };

            // memoryCache.Set(key: refreshTokenKey, value: newRefreshTokenObject, absoluteExpirationRelativeToNow: TimeSpan.FromDays(1));

            // return Results.Ok(new PostTokenResponse
            // {
            //     AccessToken = newAccessToken,
            //     TokenType = TokenTypes.Bearer,
            //     ExpiresIn = 600,
            //     RefreshToken = newRefreshToken
            // });
        }

        if (request.GrantType != "authorization_code")
        {
            return Results.BadRequest(new PostTokenError
            {
                Error = PostTokenErrorCode.UnsupportedGrantType.GetDescription(),
                ErrorDescription = "Only authorization_code grant type is supported"
            });
        }

        if (string.IsNullOrEmpty(request.GrantType) || string.IsNullOrEmpty(request.Code) || string.IsNullOrEmpty(request.RedirectUri))
        {
            return Results.BadRequest(new PostTokenError
            {
                Error = PostTokenErrorCode.InvalidRequest.GetDescription(),
                ErrorDescription = "Missing required parameters"
            });
        }
        
        httpContext.Request.EnableBuffering();
        httpContext.Request.Body.Position = 0;

        var allowedParameters = new HashSet<string> { "grant_type", "code", "redirect_uri", "client_id" };
        using var doc = JsonDocument.Parse(httpContext.Request.Body);
        var parameters = doc.RootElement.EnumerateObject().Select(p => p.Name).ToList();
        var extraParameters = parameters.Where(p => !allowedParameters.Contains(p)).ToList();
        if (extraParameters.Any())
        {
            return Results.BadRequest(new PostTokenError
            {
                Error = PostTokenErrorCode.InvalidRequest.GetDescription(),
                ErrorDescription = $"Invalid request: contains extra parameters: {string.Join(", ", extraParameters)}"
            });
        }

        var authcodeCacheKey = $"{CacheKeys.AuthorisationCodePrefix}{request.Code}";
        var authCode = memoryCache.Get<AuthorisationCode>(authcodeCacheKey);
        if (authCode == null)
        {
            return Results.BadRequest(new PostTokenError
            { 
                Error = PostTokenErrorCode.InvalidGrant.GetDescription(), 
                ErrorDescription = "The provided authorisation grant (e.g., authorisation code, resource owner credentials) or refresh token is invalid, expired, revoked, does not match the redirection URI used in the authorisation request, or was issued to another client."
            });
        }

        if (DateTime.UtcNow > authCode.ExpiresAt)
        {
            return Results.BadRequest(new PostTokenError
            { 
                Error = PostTokenErrorCode.InvalidGrant.GetDescription(), 
                ErrorDescription = "The provided authorisation grant (e.g., authorisation code, resource owner credentials) or refresh token is invalid, expired, revoked, does not match the redirection URI used in the authorisation request, or was issued to another client."
            });
        }

        if (authCode.ClientId != request.ClientId)
        {
            return Results.BadRequest(new PostTokenError
            { 
                Error = PostTokenErrorCode.InvalidGrant.GetDescription(), 
                ErrorDescription = "The provided authorisation grant (e.g., authorisation code, resource owner credentials) or refresh token is invalid, expired, revoked, does not match the redirection URI used in the authorisation request, or was issued to another client."
            });
        }

        if (authCode.RedirectUri != request.RedirectUri)
        {
            return Results.BadRequest(new PostTokenError
            { 
                Error = PostTokenErrorCode.InvalidGrant.GetDescription(), 
                ErrorDescription = "The provided authorisation grant (e.g., authorisation code, resource owner credentials) or refresh token is invalid, expired, revoked, does not match the redirection URI used in the authorisation request, or was issued to another client."
            });
        }

        if (string.IsNullOrWhiteSpace(request.UserId))
        {
            return Results.BadRequest(new PostTokenError
            {
                Error = PostTokenErrorCode.InvalidRequest.GetDescription(),
                ErrorDescription = "User id must be supplied"
            });
        }

        var client = await dbContext.Clients.FindAsync(authCode.ClientId);
        if (client == null)
        {
            return Results.BadRequest(new PostTokenError
            {
                Error = PostTokenErrorCode.InvalidRequest.GetDescription(),
                ErrorDescription = "Client not found"
            });
        }

        if (client.RequirePkce)
        {
            if (string.IsNullOrWhiteSpace(request.CodeVerifier))
            {
                return Results.BadRequest(new PostTokenError
                {
                    Error = PostTokenErrorCode.InvalidRequest.GetDescription(),
                    ErrorDescription = "Code verifier is required"
                });
            }

            if (!PkceVerifier.ValidatePkce(request.CodeVerifier, authCode.CodeChallenge))
            {
                // TODO: Check if this error description is correct. I just made it vague for now.
                return Results.BadRequest(new PostTokenError
                {
                    Error = PostTokenErrorCode.InvalidGrant.GetDescription(),
                    ErrorDescription = "The provided authorisation grant (e.g., authorisation code, resource owner credentials) or refresh token is invalid, expired, revoked, does not match the redirection URI used in the authorisation request, or was issued to another client."
                });
            }
        }

        var now = DateTime.UtcNow;

        var accessTokenPayload = new Dictionary<string, object>
        { 
            { JwtClaims.Subject, request.UserId },
            { JwtClaims.Issuer, jwtOptions.Value.Issuer },
            { JwtClaims.Audience, client.Audiences.Select(aud => aud.Name).ToArray() },
            { JwtClaims.Expiration, now.AddSeconds(client.AccessTokenLifetimeInSeconds).ToString() },
            { JwtClaims.IssuedAt, now.ToString() },
            { JwtClaims.JwtId, Guid.NewGuid().ToString() },
            { JwtClaims.NotBefore, now.ToString() },
            { JwtClaims.Scope, authCode.Scopes }
        };
        var accessToken = tokenGenerator.Generate(algo: jwtOptions.Value.AccessTokenSigningAlgorithm, payload: accessTokenPayload);

        memoryCache.Remove(authcodeCacheKey);

        logger.LogInformation("Token request handled successfully for client {ClientId}", request.ClientId);

        return Results.Ok(new PostTokenResponse
        {
            AccessToken = accessToken,
            TokenType = TokenTypes.Bearer,
            ExpiresIn = 3600
        });
    }
}
