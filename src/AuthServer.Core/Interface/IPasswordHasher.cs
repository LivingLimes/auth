namespace AuthServer.Core.Interface;

public interface IPasswordHasher
{
    string HashPassword(string password, out string passwordSalt, out int passwordIterations);
    bool VerifyHashedPassword(string password, string hashedPassword, string hashedPasswordSalt, int hashPasswordIterations);
}
