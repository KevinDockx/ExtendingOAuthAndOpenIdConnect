using ExtendingOAuthAndOpenIDConnect.WebClient.Controllers;
using ExtendingOAuthAndOpenIDConnect.WebClient.OidcEvents;
using ExtendingOAuthAndOpenIDConnect.WebClient.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using ExtendingOAuthAndOpenIDConnect.WebClient.Helpers;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();

// IDP client
builder.Services.AddHttpClient("IdpClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:5001");
});

// API client
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7015");
});

// register the generator for private key jwt tokens (used for client authentication)
builder.Services.AddSingleton<ITokenGenerator, TokenGenerator>();
// register the auth request signer
builder.Services.AddSingleton<IAuthorizationRequestSigner, AuthorizationRequestSigner>();  

// register OidcEvent types
builder.Services.AddTransient<WebClientDemoJwtEvents>();
builder.Services.AddTransient<WebClientDemoJarEvents>();
builder.Services.AddTransient<WebClientDemoJwtAndJarEvents>();
 
// auth Schemes 
builder.Services.AddAuthentication("ApplicationCookieScheme")
.AddCookie("ApplicationCookieScheme")
.AddOpenIdConnect("CodeFlowScheme", options =>
{
    options.SignInScheme = "ApplicationCookieScheme";
    options.Authority = "https://localhost:5001";
    // When registering multiple OIDC schemes they should all have a
    // unique callbackpath to avoid "Unable to unprotect the message.State" errors. 
    // Those errors happen when one scheme registration is trying to unprotect the 
    // state set by another scheme registration
    options.CallbackPath = "/signin-codeflow";
    options.ResponseType = "code";
    options.GetClaimsFromUserInfoEndpoint = true;
    options.ClientId = "webclientdemo";
    options.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
    options.Scope.Add("demoapi.fullaccess");
    options.SaveTokens = true;
    options.TokenValidationParameters = new()
    {
        NameClaimType = "given_name",
        RoleClaimType = "role",
    };
})
.AddOpenIdConnect("CodeFlowWithPrivateKeyJWTScheme", options =>
{
    options.SignInScheme = "ApplicationCookieScheme";
    options.Authority = "https://localhost:5001";
    options.CallbackPath = "/signin-codeflowprivatekeyjwt";
    options.ResponseType = "code";
    options.GetClaimsFromUserInfoEndpoint = true;
    options.ClientId = "webclientdemojwt";
    options.Scope.Add("demoapi.fullaccess");
    options.SaveTokens = true;
    options.TokenValidationParameters = new()
    {
        NameClaimType = "given_name",
        RoleClaimType = "role",
    };
    options.EventsType = typeof(WebClientDemoJwtEvents);
})
.AddOpenIdConnect("CodeFlowWithJARScheme", options =>
{
    options.SignInScheme = "ApplicationCookieScheme";
    options.Authority = "https://localhost:5001";
    options.CallbackPath = "/signin-codeflowjar";
    options.ResponseType = "code";
    options.GetClaimsFromUserInfoEndpoint = true;
    options.ClientId = "webclientdemojar";
    options.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
    options.Scope.Add("demoapi.fullaccess");
    options.SaveTokens = true;
    options.TokenValidationParameters = new()
    {
        NameClaimType = "given_name",
        RoleClaimType = "role",
    };
    options.EventsType = typeof(WebClientDemoJarEvents);
})
.AddOpenIdConnect("CodeFlowWithPrivateKeyJWTandJARScheme", options =>
{
    options.SignInScheme = "ApplicationCookieScheme";
    options.Authority = "https://localhost:5001";
    options.CallbackPath = "/signin-codeflowprivatekeyjwtandjar";
    options.ResponseType = "code";
    options.GetClaimsFromUserInfoEndpoint = true;
    options.ClientId = "webclientdemojwtandjar";
    options.Scope.Add("demoapi.fullaccess");
    options.SaveTokens = true;
    options.TokenValidationParameters = new()
    {
        NameClaimType = "given_name",
        RoleClaimType = "role",
    };
    options.EventsType = typeof(WebClientDemoJwtAndJarEvents);
})
.AddOpenIdConnect("CodeFlowWithTokenEncryptionScheme", options =>
{
    options.SignInScheme = "ApplicationCookieScheme";
    options.Authority = "https://localhost:5001";
    options.CallbackPath = "/signin-codeflowtokenencryption";
    options.ResponseType = "code";
    options.GetClaimsFromUserInfoEndpoint = true;
    options.ClientId = "webclientdemotokenencryption";
    options.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
    options.Scope.Add("demoapi.fullaccess");
    options.SaveTokens = true;
    options.TokenValidationParameters = new()
    {
        NameClaimType = "given_name",
        RoleClaimType = "role",
        TokenDecryptionKey = new X509SecurityKey(
            new X509Certificate2("democert.pfx", "demopassword"))
    };
})
.AddOpenIdConnect("CodeFlowWithDPoPScheme", options =>
{
    options.SignInScheme = "ApplicationCookieScheme";
    options.Authority = "https://localhost:5001";
    options.CallbackPath = "/signin-codeflowdpop";
    options.ResponseType = "code";
    options.GetClaimsFromUserInfoEndpoint = true;
    options.ClientId = "webclientdemodpop";
    options.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
    options.Scope.Add("demoapi.fullaccess");
    options.SaveTokens = true;
    options.TokenValidationParameters = new()
    {
        NameClaimType = "given_name",
        RoleClaimType = "role"
    };
});

// enable access token management (for DPoP support) 
builder.Services.AddClientCredentialsTokenManagement()
    .AddClient("dpopclient_clientdefinition", client =>
    {
        client.TokenEndpoint = "https://localhost:5001/connect/token";
        client.DPoPJsonWebKey = JwkHelper.GenerateJsonWebKey();
        client.ClientId = "dpopclient";
        client.ClientSecret = "49C1A7E1-0C79-4A89-A3D6-A37998FB86B0";
        client.Scope = "demoapi.fullaccess";
    });

// API client with DPoP support
builder.Services.AddHttpClient("ApiClient_DPoP", client =>
{
    client.BaseAddress = new Uri("https://localhost:7015");
}).AddClientCredentialsTokenHandler("dpopclient_clientdefinition");


builder.Services.AddOpenIdConnectAccessTokenManagement(options =>
{ 
    options.ChallengeScheme = "CodeFlowWithDPoPScheme";
    options.DPoPJsonWebKey = JwkHelper.GenerateJsonWebKey();
});

// API client with DPoP support, user based
builder.Services.AddHttpClient("ApiClient_DPoP_UserBased", client =>
{
    client.BaseAddress = new Uri("https://localhost:7015");
}).AddUserAccessTokenHandler();
 
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=OidcSamples}/{action=Index}/{id?}");

app.Run();
 