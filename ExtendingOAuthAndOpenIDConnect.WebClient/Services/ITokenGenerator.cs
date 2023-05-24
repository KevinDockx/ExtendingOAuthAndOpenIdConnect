namespace ExtendingOAuthAndOpenIDConnect.WebClient.Services;

public interface ITokenGenerator
{
    public string GenerateSignedToken(string clientId, string audience);
}
