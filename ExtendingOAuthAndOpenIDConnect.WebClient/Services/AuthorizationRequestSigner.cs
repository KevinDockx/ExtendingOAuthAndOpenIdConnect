using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

namespace ExtendingOAuthAndOpenIDConnect.WebClient.Services;

public class AuthorizationRequestSigner : IAuthorizationRequestSigner
{
    public string SignRequest(OpenIdConnectMessage message, string clientId, string audience)
    {
        var certificate = new X509Certificate2("democert.pfx", "demopassword");
        var now = DateTime.UtcNow;

        var claims = new List<Claim>();
        foreach (var parameter in message.Parameters)
        {
            claims.Add(new Claim(parameter.Key, parameter.Value));
        }

        var token = new JwtSecurityToken(
            clientId,
            audience,
            claims,
            now,
            now.AddMinutes(1),
            new SigningCredentials(
                    new X509SecurityKey(certificate),
                    SecurityAlgorithms.RsaSha256
                )
        );

        var tokenHandler = new JwtSecurityTokenHandler();
        tokenHandler.OutboundClaimTypeMap.Clear();
        return tokenHandler.WriteToken(token);
    }
}
