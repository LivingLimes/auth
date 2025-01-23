namespace AuthServer.Core;

public class RedirectUris
{
    public RedirectUri[] Value { get; init; } = Array.Empty<RedirectUri>();

    private RedirectUris(string[] uris)
    {
        Value = uris.Select(RedirectUri.Create).ToArray();
    }

    public static bool CanCreate(string[] uris)
    {
        if (uris.Length >= 100)
        {
            return false;
        }

        return uris.All(RedirectUri.CanCreate);
    }

    public static RedirectUris Create(string[] uris)
    {
        if (!CanCreate(uris))
        {
            throw new Exception($"Supplied redirect URIs: '{string.Join(", ", uris)}' are invalid.");
        }

        return new RedirectUris(uris);
    }

    public bool IsAllowed(string uri)
    {
        return Value.Any(ru => ru.Value == uri);
    }
}
