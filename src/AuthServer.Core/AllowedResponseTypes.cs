namespace AuthServer.Core;

public class AllowedResponseTypes
{
    private static HashSet<string> validResponseTypeSet = new HashSet<string>
    {
        "code",
    };

    public AllowedResponseType[] Value { get; private set; } = Array.Empty<AllowedResponseType>();

    private AllowedResponseTypes()
    {
    }

    private AllowedResponseTypes(string[] responseTypes)
    {
        Value = responseTypes.Select(responseType => AllowedResponseType.Create(EnumMethods.ParseFromDescription<ResponseType>(responseType))).ToArray();
    }

    public static bool CanCreate(string[] responseTypes)
    {
        if (!responseTypes.Any())
        {
            return false;
        }
        if (!responseTypes.All(validResponseTypeSet.Contains))
        {
            return false;
        }
        return true;
    }

    public static AllowedResponseTypes Create(string[] responseTypes)
    {
        if (!CanCreate(responseTypes))
        {
            throw new Exception($"Supplied response types: '{string.Join(", ", responseTypes)}' are invalid.");
        }
        return new AllowedResponseTypes(responseTypes);
    }
}
