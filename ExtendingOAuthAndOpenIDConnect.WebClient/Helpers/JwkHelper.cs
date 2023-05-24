using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text.Json;

namespace ExtendingOAuthAndOpenIDConnect.WebClient.Helpers;

public static class JwkHelper
{
    public static string GenerateJsonWebKey()
    {
        var rsaKey = new RsaSecurityKey(RSA.Create(2048));
        var jsonWebKey = JsonWebKeyConverter.ConvertFromSecurityKey(rsaKey);
        jsonWebKey.Alg = "PS256";
        return JsonSerializer.Serialize(jsonWebKey);
    }
}
