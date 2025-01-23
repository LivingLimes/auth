namespace AuthServer.Core;

public class AllowedResponseType
{
    public Guid ClientId { get; init; }
    public ResponseType ResponseType { get; set; }

    private AllowedResponseType() { }

    private AllowedResponseType(ResponseType allowedResponseType)
    {
        ResponseType = allowedResponseType;
    }

    public static AllowedResponseType Create(ResponseType responseType)
    {
        return new AllowedResponseType(responseType);
    }
}