using ExtendingOAuthAndOpenIDConnect.WebClient.Services;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace ExtendingOAuthAndOpenIDConnect.WebClient.OidcEvents;

public class WebClientDemoJarEvents : OpenIdConnectEvents
{
    private readonly IAuthorizationRequestSigner _authorizationRequestSigner; 

    public WebClientDemoJarEvents(IAuthorizationRequestSigner authorizationRequestSigner)
    {
        _authorizationRequestSigner = authorizationRequestSigner ??
            throw new ArgumentNullException(nameof(authorizationRequestSigner));
    }
 
    public override Task RedirectToIdentityProvider(RedirectContext context)
    {
        var request = _authorizationRequestSigner.SignRequest(
            context.ProtocolMessage,
            context.ProtocolMessage.ClientId, 
            "https://localhost:5001");
        var clientId = context.ProtocolMessage.ClientId;
        var redirectUri = context.ProtocolMessage.RedirectUri;

        context.ProtocolMessage.Parameters.Clear();
        context.ProtocolMessage.ClientId = clientId;
        context.ProtocolMessage.RedirectUri = redirectUri;
        context.ProtocolMessage.SetParameter("request", request);

        return Task.CompletedTask;
    }
}