namespace AuthServer.Core.Interface;

public interface IJwtGenerator
{
    public string Generate(string algo, Dictionary<string, object> payload);
}
