using System.Diagnostics.CodeAnalysis;

namespace AuthServer.Core;

public class Client : BaseEntity
{
    public required string Name { get; init; }

#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0052 // Remove unread private members
    private ICollection<AllowedGrantType> _allowedGrantTypes = new List<AllowedGrantType>();
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore IDE0044 // Add readonly modifier
    public required AllowedGrantTypes AllowedGrantTypes { get; init; }

#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0052 // Remove unread private members
    private ICollection<RedirectUri> _redirectUris = new List<RedirectUri>();
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore IDE0044 // Add readonly modifier
    public required RedirectUris RedirectUris { get; init; }

    public TokenEndpointAuthMethod TokenEndpointAuthMethod { get; init; }

#pragma warning disable IDE0044 // Add readonly modifier
#pragma warning disable IDE0052 // Remove unread private members
    private ICollection<AllowedResponseType> _allowedResponseTypes = new List<AllowedResponseType>();
#pragma warning restore IDE0052 // Remove unread private members
#pragma warning restore IDE0044 // Add readonly modifier
    public required AllowedResponseTypes AllowedResponseTypes { get; init; }

    public required bool RequirePkce { get; init; }
    public required int AccessTokenLifetimeInSeconds {get; init;}

    private ICollection<Audience> _audiences = new List<Audience>();
    public required ICollection<Audience> Audiences { get; init; }

    private Client() { }

    [SetsRequiredMembers]
    private Client(string name, AllowedGrantTypes allowedGrantTypes, RedirectUris redirectUris, AllowedResponseTypes allowedResponseTypes, TokenEndpointAuthMethod tokenEndpointAuthMethod, bool requirePkce, int accessTokenLifetimeInSeconds, IEnumerable<Audience> audiences)
    {
        Name = name;
        
        _allowedGrantTypes = allowedGrantTypes.Value;
        AllowedGrantTypes = allowedGrantTypes;

        _redirectUris = redirectUris.Value;
        RedirectUris = redirectUris;

        _allowedResponseTypes = allowedResponseTypes.Value;
        AllowedResponseTypes = allowedResponseTypes;

        TokenEndpointAuthMethod = tokenEndpointAuthMethod;
        RequirePkce = requirePkce;
        AccessTokenLifetimeInSeconds = accessTokenLifetimeInSeconds;

        _audiences = audiences.ToList();
        Audiences = _audiences;
    }

    public static Client Create(string name, AllowedGrantTypes allowedGrantTypes, RedirectUris redirectUris, AllowedResponseTypes allowedResponseTypes, TokenEndpointAuthMethod tokenEndpointAuthMethod, bool requirePkce, int accessTokenLifetimeInSeconds, IEnumerable<Audience> audiences)
    {
        return new Client(name, allowedGrantTypes, redirectUris, allowedResponseTypes, tokenEndpointAuthMethod, requirePkce, accessTokenLifetimeInSeconds, audiences.ToList());
    }

}