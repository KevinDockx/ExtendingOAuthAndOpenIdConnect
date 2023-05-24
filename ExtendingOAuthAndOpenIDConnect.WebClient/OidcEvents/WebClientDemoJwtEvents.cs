using ExtendingOAuthAndOpenIDConnect.WebClient.Services;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace ExtendingOAuthAndOpenIDConnect.WebClient.OidcEvents;

public class WebClientDemoJwtEvents : OpenIdConnectEvents
{
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IHttpClientFactory _httpClientFactory;

    public WebClientDemoJwtEvents(ITokenGenerator tokenGenerator, 
        IHttpClientFactory httpClientFactory)
    {
        _tokenGenerator = tokenGenerator ?? 
            throw new ArgumentNullException(nameof(tokenGenerator));
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
                "webclientdemojwt",
                discoveryDocumentResponse.TokenEndpoint);

        context.TokenEndpointRequest.ClientAssertion = signedToken;                  
    } 
} 