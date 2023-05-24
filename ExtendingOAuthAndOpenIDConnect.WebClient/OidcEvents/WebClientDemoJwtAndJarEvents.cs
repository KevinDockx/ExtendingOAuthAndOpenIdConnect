using ExtendingOAuthAndOpenIDConnect.WebClient.Services;
using IdentityModel.Client;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace ExtendingOAuthAndOpenIDConnect.WebClient.OidcEvents;

public class WebClientDemoJwtAndJarEvents : OpenIdConnectEvents
{
    private readonly IAuthorizationRequestSigner _authorizationRequestSigner; 
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IHttpClientFactory _httpClientFactory;

    public WebClientDemoJwtAndJarEvents(
        ITokenGenerator tokenGenerator,
        IAuthorizationRequestSigner authorizationRequestSigner,
        IHttpClientFactory httpClientFactory)
    {
        _tokenGenerator = tokenGenerator ??
            throw new ArgumentNullException(nameof(tokenGenerator));
        _authorizationRequestSigner = authorizationRequestSigner ??
            throw new ArgumentNullException(nameof(authorizationRequestSigner));
        _httpClientFactory = httpClientFactory ??
            throw new ArgumentNullException(nameof(httpClientFactory));
    }

    public override async Task AuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
    {
        var idpClient = _httpClientFactory.CreateClient("IdpClient");
        var discoveryDocumentResponse = await idpClient.GetDiscoveryDocumentAsync();
        if (discoveryDocumentResponse.IsError)
        {
            throw new Exception(discoveryDocumentResponse.Error);
        }

        context.TokenEndpointRequest.ClientAssertionType =
            OidcConstants.ClientAssertionTypes.JwtBearer;
        var signedToken = _tokenGenerator.GenerateSignedToken( 
                "webclientdemojwtandjar",
                discoveryDocumentResponse.TokenEndpoint);

        context.TokenEndpointRequest.ClientAssertion = signedToken;
    }

    public override Task RedirectToIdentityProvider(RedirectContext context)
    {
        var singedRequest = _authorizationRequestSigner.SignRequest(
            context.ProtocolMessage,
            context.ProtocolMessage.ClientId,
            "https://localhost:5001");
        var clientId = context.ProtocolMessage.ClientId;
        var redirectUri = context.ProtocolMessage.RedirectUri;

        context.ProtocolMessage.Parameters.Clear();
        context.ProtocolMessage.ClientId = clientId;
        context.ProtocolMessage.RedirectUri = redirectUri;
        context.ProtocolMessage.SetParameter("request", singedRequest);

        return Task.CompletedTask;
    }
}
