using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ExtendingOAuthAndOpenIDConnect.WebClient.Services;

public interface IAuthorizationRequestSigner
{
    string SignRequest(OpenIdConnectMessage message, string clientId, string idpUrl);
}