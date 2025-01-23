namespace AuthServer.Core;

public class RedirectUri
{
    public Guid ClientId { get; init; }
    public string Value { get; init; } = string.Empty;

    private RedirectUri() { }

    private RedirectUri(string uri)
    {
        Value = uri;
    }

    public static bool CanCreate(string uri)
    {
        // It is important for the URI to be absolute to ensure that it is a complete and valid address.
        // Absolute URIs include the scheme, host, and path, which are necessary for the redirection process.
        // This helps prevent potential security issues and ensures that the redirection targets are well-defined and reachable.
        if (!Uri.TryCreate(uri, UriKind.Absolute, out var result))
        {
            return false;
        }

        // The RFC allows http connections, but for our application, we are enforcing HTTPS connections only aside from localhost connections for testing
        if (result.Scheme != "https" && !IsLocalhost(result))
        {
            return false;
        }

        if (result.Fragment != string.Empty)
        {
            return false;
        }

        if (result.Port < 0 || result.Port > 65535)
        {
            return false;
        }

        if (uri.Contains("*"))
        {
            return false;
        }

        return true;
    }

    // When accessing http://localhost:

    // 1. DNS Resolution (only for "localhost", not needed for 127.0.0.1)
    // System checks hosts file/DNS resolver
    // Maps "localhost" to loopback IP (127.0.0.1)

    // 2. Network Stack Processing
    // Request enters OS network stack
    // OS sees destination is loopback address (127.0.0.1)
    // Instead of forwarding to network interface, OS routes packet back through stack
    // No physical network hardware is involved - packet loops back internally

    // 3. Local Server Processing 
    // If server is listening on specified port (e.g. port 80)
    // Server process receives request through loopback interface
    // Processes request and generates response
    // Response sent back through same loopback path
    // Again never touches physical network - pure internal routing

    // Key Points:
    // Loopback interface is virtual - provides internal communication path
    // Traffic never leaves your machine so this works without an internet connection
    // The same applies to ::1, except that this is an ipv6 IP address
    private static bool IsLocalhost(Uri uri)
    {
        return uri.Scheme == "http" && (uri.Host == "localhost" || uri.Host == "127.0.0.1" || uri.Host == "::1");
    }

    public static RedirectUri Create(string uri)
    {
        if (!CanCreate(uri))
        {
            throw new Exception($"Supplied redirect URI: '{uri}' is invalid.");
        }

        return new RedirectUri(uri);
    }
}
