using Duende.AccessTokenManagement;
using Duende.AccessTokenManagement.OpenIdConnect;
using ExtendingOAuthAndOpenIDConnect.WebClient.Services;
using IdentityModel;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Security.Claims;

namespace ExtendingOAuthAndOpenIDConnect.WebClient.Controllers;

public class OidcSamplesController : Controller
{
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IClientCredentialsTokenManagementService _clientCredentialsTokenManagementService;
    private readonly IUserTokenManagementService _userTokenManagementService;
    private static string _lastChallengedScheme = string.Empty;

    public OidcSamplesController(ITokenGenerator tokenGenerator, 
        IHttpClientFactory httpClientFactory,
        IClientCredentialsTokenManagementService clientCredentialsTokenManagementService,
        IUserTokenManagementService userTokenManagementService)
    {
        _tokenGenerator = tokenGenerator ?? 
            throw new ArgumentNullException(nameof(tokenGenerator));
        _httpClientFactory = httpClientFactory ?? 
            throw new ArgumentNullException(nameof(httpClientFactory));
        _clientCredentialsTokenManagementService = clientCredentialsTokenManagementService ?? 
            throw new ArgumentNullException(nameof(clientCredentialsTokenManagementService));
        _userTokenManagementService = userTokenManagementService ?? 
            throw new ArgumentNullException(nameof(userTokenManagementService));
    }

    public async Task<IActionResult> Index()
    {  
        return View();
    }

    public async Task<IActionResult> CallApi()
    {  
        var client = _httpClientFactory.CreateClient("ApiClient");
        var accessToken = await HttpContext.GetTokenAsync(
            OpenIdConnectParameterNames.AccessToken);
        if (accessToken != null)
        {
            client.SetBearerToken(accessToken);
        }

        ViewBag.ResultFromApiCall = await client.GetStringAsync("api/claims"); 
        return View("ApiResult");
    }

    public async Task<ActionResult> CallApiWithPrivateKeyJwtAuthenticationClientCredentialsFlow()
    {
        var idpClient = _httpClientFactory.CreateClient("IdpClient");
        var discoveryDocumentResponse = await idpClient.GetDiscoveryDocumentAsync();
        if (discoveryDocumentResponse.IsError)
        {
            throw new Exception(discoveryDocumentResponse.Error);
        }

        var signedToken = _tokenGenerator.GenerateSignedToken(
            "systemjwtclient", 
            discoveryDocumentResponse.TokenEndpoint);
         
        var tokenResponse = await idpClient.RequestClientCredentialsTokenAsync(
            new ()
            {
                // default ClientCredentialStyle = via auth header, 
                // but client assertions are passed via the body
                ClientCredentialStyle = ClientCredentialStyle.PostBody,
                Address = discoveryDocumentResponse.TokenEndpoint,
                ClientId = "systemjwtclient",
                Scope = "demoapi.fullaccess",

                ClientAssertion =
                    {
                        Type = OidcConstants.ClientAssertionTypes.JwtBearer,
                        Value = signedToken
                    }
            });

        if (tokenResponse.IsError)
        {
            throw new Exception(tokenResponse.Error);
        }

        // call API with the access token
        var apiClient = _httpClientFactory.CreateClient("ApiClient");
        apiClient.SetBearerToken(tokenResponse.AccessToken);

        ViewBag.ResultFromApiCall = await apiClient.GetStringAsync("api/claims");
        return View("ApiResult");
    }
         
    public async Task<ActionResult> CallApiWithDPoPClientCredentialsFlow()
    { 
        // manually get an access token
        var accessToken = await _clientCredentialsTokenManagementService
            .GetAccessTokenAsync("dpopclient_clientdefinition");
        // now you can use that token, eg: to set it as bearer token 
        // on outgoing requests to the API
         
        // OR let the access token management package fully handle it. 
        // (nothing else needed, the underlying handler takes care of everything)
        var apiClient = _httpClientFactory.CreateClient("ApiClient_DPoP"); 
        ViewBag.ResultFromApiCall = await apiClient.GetStringAsync("api/claims"); 
        return View("ApiResult");
    }


    public async Task<ActionResult> CallApiWithDPoPCodeFlow()
    { 
        var apiClient = _httpClientFactory.CreateClient("ApiClient_DPoP_UserBased");
        ViewBag.ResultFromApiCall = await apiClient.GetStringAsync("api/claims");
        return View("ApiResult");
    }



    /// <summary>
    /// Not authenticated?  Trigger auth & return to the caller eventually. 
    /// </summary> 
    public async Task<IActionResult> ChallengeScheme(string schemeToChallenge)
    {
        // already logged in via this scheme?
        var result = await HttpContext.AuthenticateAsync(
            schemeToChallenge ?? _lastChallengedScheme);

        if (result?.Succeeded != true)
        {
            // nope - so challenge the scheme
            _lastChallengedScheme = schemeToChallenge;
            return Challenge(schemeToChallenge);
        }

        // Create a new ClaimsIdentity & Principal from the
        // auth result, and sign in to the application (= create the cookie)
        // so the requests after this are still authenticated. 
        var identity = new ClaimsIdentity(
             result.Principal.Identity, 
             null, 
             result.Principal.Identity.AuthenticationType,
             "given_name", 
             "role"); 
        var claimsPrincipal = new ClaimsPrincipal(identity);
   
        // sign in with the auth result's properties
        // (contains original authentication info & tokens)
        await HttpContext.SignInAsync(
            "ApplicationCookieScheme", 
            claimsPrincipal,
            result.Properties); 
        return RedirectToAction("Index"); 
    }
     
    public async Task SignOut(string schemeToSignOut)
    {        
        // Clears the  local cookie
        await HttpContext.SignOutAsync("ApplicationCookieScheme");

        // Redirects to the IDP linked to the passed-through scheme 
        // so it can clear its own session/cookie
        await HttpContext.SignOutAsync(schemeToSignOut);
    } 
}
