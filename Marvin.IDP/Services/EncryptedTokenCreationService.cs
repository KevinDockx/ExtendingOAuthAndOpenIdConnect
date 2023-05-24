using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography.X509Certificates; 
using Microsoft.AspNetCore.Authentication;
using System.IdentityModel.Tokens.Jwt;
using Duende.IdentityServer.Configuration;

namespace Marvin.IDP.Services;

// Based on Scott Brady's IdSrv4 approach
// Thank you, Scott! - https://www.scottbrady91.com/identity-server/encrypting-identity-tokens-in-identityserver4
public class EncryptedTokenCreationService : DefaultTokenCreationService
{
    public EncryptedTokenCreationService(
       ISystemClock clock,
       IKeyMaterialService keys,
       IdentityServerOptions options,
       ILogger<DefaultTokenCreationService> logger)
         : base(clock, keys, options, logger)
    { 
    }
     
    public override async Task<string> CreateTokenAsync(Token token)
    {
        if (token.Type == IdentityServerConstants.TokenTypes.IdentityToken)
        {
            var payload = await base.CreatePayloadAsync(token);

            var handler = new JsonWebTokenHandler();
            var jwe = handler.CreateToken(
                payload, 
                await Keys.GetSigningCredentialsAsync(), 
                new X509EncryptingCredentials(new X509Certificate2("democertpublickeyonly.cer")));

            return jwe;
        }

        return await base.CreateTokenAsync(token); 
    }
}
