namespace AuthServer.Core;

public class User : BaseEntity
{
    public required string Username { get; init; }
    public required string PasswordHash { get; init; }
    public required string PasswordSalt { get; init; }
    public required int PasswordIterations { get; init; }

    private User() { }

    public static User Create(string username, string passwordHash, string passwordSalt, int passwordIterations)
    {
        return new User
        {
            Username = username,
            PasswordHash = passwordHash,
            PasswordSalt = passwordSalt,
            PasswordIterations = passwordIterations,
        };
    }
}