namespace AuthServer.Core;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; } = Guid.NewGuid();
    public DateTime CreatedOn { get; protected set; } = DateTime.UtcNow;
    public DateTime UpdatedOn { get; protected set; } = DateTime.UtcNow;
    public bool SoftDelete { get; protected set; } = false;
}
