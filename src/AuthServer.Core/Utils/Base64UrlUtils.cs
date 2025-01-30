namespace AuthServer.Application.Endpoints.PostToken;

using System.Text;
using System.Text.Json;

// Base64 strings are used as some protocols can only safely handle printable ASCII characters rather than bytes. E.g. HTTP headers, JSON.
// Base64Urlstrings are a variant of Base64 that uses URL-safe characters. Specifically, it replaces characters that have special meaning in URLs:
// + represents a space in URLs
// / is used in paths
// = is used in query parameters. We trim it as it is padding and not needed.
public static class Base64UrlUtils
{
    public static string JsonToBase64UrlString<T>(T value)
    {
        var json = JsonSerializer.Serialize(value);
        var bytes = Encoding.UTF8.GetBytes(json);
        var base64Url = ToBase64Url(bytes);
        return base64Url;
    }

    public static string ToBase64Url(byte[] bytes)
    {
        var base64 = Convert.ToBase64String(bytes);
        var base64Url = base64.TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
        return base64Url;
    }
} 