using Microsoft.Extensions.Options;

namespace AuthServer.Application.Options.Jwt;

public class JwtOptionsValidator : IValidateOptions<JwtOptions>
{
    public ValidateOptionsResult Validate(string? name, JwtOptions options)
    {
        if (string.IsNullOrEmpty(options.AccessTokenSigningAlgorithm))
        {
            return ValidateOptionsResult.Fail("Jwt:AccessTokenSigningAlgorithm is required");
        }

        if (options.AccessTokenSigningAlgorithm != "RS256")
        {
            return ValidateOptionsResult.Fail("Jwt:AccessTokenSigningAlgorithm only supports RS256");
        }

        if (string.IsNullOrEmpty(options.Issuer))
        {
            return ValidateOptionsResult.Fail("Jwt:Issuer is required");
        }

        return ValidateOptionsResult.Success;
    }
}