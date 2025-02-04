using System.Security.Cryptography;
using System.Text;

public class PkceVerifier
{
    private static string CalculateCodeChallenge(string codeVerifier)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
        return Convert.ToBase64String(hash).Replace("=", "").Replace("+", "-").Replace("/", "_");
    }

    public static bool ValidatePkce(string codeVerifier, string codeChallenge)
    {
        var calculatedCodeChallenge = CalculateCodeChallenge(codeVerifier);
        return calculatedCodeChallenge.Equals(codeChallenge, StringComparison.Ordinal);
    }
}