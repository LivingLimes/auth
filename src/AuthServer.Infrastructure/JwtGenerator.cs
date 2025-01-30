namespace AuthServer.Application.Endpoints.PostToken;

using System.Security.Cryptography;
using System.Text;
using AuthServer.Core.Interface;

public class JwtGenerator : IJwtGenerator
{
    private readonly RSA _privateKey;

    public JwtGenerator(RSA privateKey)
    {
        _privateKey = privateKey;
    }
    
    public string Generate(string algo, Dictionary<string, object> payload)
    {
        var header = new Dictionary<string, string>
        {
            { "alg", algo },
            { "typ", "JWT" },
        };

        var headerBase64UrlString = Base64UrlUtils.JsonToBase64UrlString(header);
        var payloadBase64UrlString = Base64UrlUtils.JsonToBase64UrlString(payload);

        var dataToSign = $"{headerBase64UrlString}.{payloadBase64UrlString}";
        var dataToSignBytes = Encoding.UTF8.GetBytes(dataToSign);
        var signature = _privateKey.SignData(dataToSignBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        var signatureBase64UrlString = Base64UrlUtils.ToBase64Url(signature);

        return $"{headerBase64UrlString}.{payloadBase64UrlString}.{signatureBase64UrlString}";
    }
}
