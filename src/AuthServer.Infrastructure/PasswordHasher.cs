namespace AuthServer.Infrastructure;

using AuthServer.Core.Interface;
using System.Security.Cryptography;
using System.Diagnostics;

public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 32;
    private const int HashSize = 32;
    private const int iterations = 10_000;
    private const int MinVerificationTime = 1_000;
    public string HashPassword(string password, out string passwordSalt, out int passwordIterations)
    {        
        if (string.IsNullOrEmpty(password))
        {
            throw new Exception("Password cannot be empty");
        }

        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName.SHA256, HashSize);

        passwordSalt = Convert.ToBase64String(salt);
        passwordIterations = iterations;
        return Convert.ToBase64String(hash);
    }

    public bool VerifyHashedPassword(string password, string hashedPassword, string hashedPasswordSalt, int hashPasswordIterations)
    {
        var startTime = Stopwatch.StartNew();

        try
        {
            var hashPassword = Rfc2898DeriveBytes.Pbkdf2(password, Convert.FromBase64String(hashedPasswordSalt), hashPasswordIterations, HashAlgorithmName.SHA256, HashSize);

            return CryptographicOperations.FixedTimeEquals(hashPassword, Convert.FromBase64String(hashedPassword));
        } catch 
        {
            return false;
        }
        finally
        {
            var elapsedTime = startTime.ElapsedMilliseconds;
            if (elapsedTime < MinVerificationTime)
            {
                Thread.Sleep((int)(MinVerificationTime - elapsedTime));
            }
        }
    }
}