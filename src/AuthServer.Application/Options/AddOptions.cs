namespace AuthServer.Application.Options;

using AuthServer.Application.Options.Jwt;
using Microsoft.Extensions.Options;


public static partial class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfigOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.AddSingleton<IValidateOptions<JwtOptions>, JwtOptionsValidator>();

        return services;
    }
}