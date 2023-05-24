using Duende.IdentityServer;
using Duende.IdentityServer.Models;
using static Duende.IdentityServer.IdentityServerConstants;

namespace Marvin.IDP;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
       new IdentityResource[]
       {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email()
       };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
                new ApiScope("demoapi.fullaccess", "Full access")
        };

    public static IEnumerable<ApiResource> ApiResources =>
        new List<ApiResource>
       {         
                // allows full access (via scope) to the API
                new ApiResource(
                    "demoapi", 
                    "The demo API", 
                    new[] { "profile", "email" })
                    {
                        Scopes = { "demoapi.fullaccess" }
                    }
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
           
            // code flow + PKCE 
            new Client
            {
                ClientId = "webclientdemo",
                ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = { "https://localhost:7119/signin-codeflow" },
                FrontChannelLogoutUri = "https://localhost:7119/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:7119/signout-callback-oidc" },

                AllowedScopes = { "openid", "profile", "email", "demoapi.fullaccess" }
            }, 
            // client credentials flow with private key JWT auth
            new Client
            {
                ClientId = "systemjwtclient",
                ClientSecrets =
                    {
                        new Secret
                        {
                            Type = IdentityServerConstants.SecretTypes.X509CertificateBase64,
                            Value = "MIIDEDCCAfigAwIBAgIQFGwbjjyAs7FKzHhNUVfjbTANBgkqhkiG9w0BAQUFADAb\r\nMRkwFwYDVQQDDBBEZW1vIEpXVCBTaWduaW5nMB4XDTIzMDUwODA4NDYzN1oXDTI0\r\nMDUwODA5MDYzN1owGzEZMBcGA1UEAwwQRGVtbyBKV1QgU2lnbmluZzCCASIwDQYJ\r\nKoZIhvcNAQEBBQADggEPADCCAQoCggEBAO3Ju83nWv97XXXAT3/f23Wwm2z7YVq9\r\nJJLOXsTUgnsQ7+JPPXpi/IVqrfDS8EIifbnjc5s8AI2sHUApurR1BLN0QBSnrXAr\r\nUGxGgyCuF7NjLnc9Q7f/kKhplokvFEqBv9DzsppDK67LCBnToEfkiTHkJgydYIfW\r\nApP42wMTDHizzWf3uZBOvF6QR6cKiQSLMUMQdIQ9HBl3XRJFIM/4iMpG2HmLLTJ+\r\nao/xWwlSpxThUuptvrad6qa5l6r8s14WYyfNq51G8Dq7NELymgrwgFiVH9GeB2XI\r\n/L2UxzsgtErLSdBuANZrl5V4QGrzuPN3QTcBmAe3ssUDgR3/7+yPl4UCAwEAAaNQ\r\nME4wDgYDVR0PAQH/BAQDAgWgMB0GA1UdJQQWMBQGCCsGAQUFBwMCBggrBgEFBQcD\r\nATAdBgNVHQ4EFgQUmo3NpTm88OuQdY/fyv9eVh1dJlAwDQYJKoZIhvcNAQEFBQAD\r\nggEBAA53S2/aPUeraUhxSiC8f8Nc3RZAaV4AcIJ2MkktTm64kKXHrGp/zt/D+zRf\r\nyTbe8XQAYxYOv8IyplRMBMPhGH7hjXIQPlTQX84lTA0odlGVUsIs57FB9JtRE0kQ\r\nRGqUzD/GGH8Uh8xxUvgiNuWAqWMktVLErYddQy41BiF2FU1qxZxc/I1Yr69XhYcV\r\nVRB9IS0mKT6dp1Zx2mP3krrhWq5dleuFu1dPTFQ6Ocy9fr1OXcYUgWvImz0BDn4l\r\nvOWQv+Py0QeIkHO9ZypYXwQBwOND5iGu8hElnM5mlWr5xVBFg1QFYnV5J7kpkWb4\r\nPixAHjDjcI+DCike5FYt5QVkXo0="
                        }
                    },
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = { "demoapi.fullaccess" }
            },          
            // code flow + PKCE + private key JWT auth
            new Client
            {
                ClientId = "webclientdemojwt",
                ClientSecrets =
                    {
                        new Secret
                        {
                            Type = IdentityServerConstants.SecretTypes.X509CertificateBase64,
                            Value = "MIIDEDCCAfigAwIBAgIQFGwbjjyAs7FKzHhNUVfjbTANBgkqhkiG9w0BAQUFADAb\r\nMRkwFwYDVQQDDBBEZW1vIEpXVCBTaWduaW5nMB4XDTIzMDUwODA4NDYzN1oXDTI0\r\nMDUwODA5MDYzN1owGzEZMBcGA1UEAwwQRGVtbyBKV1QgU2lnbmluZzCCASIwDQYJ\r\nKoZIhvcNAQEBBQADggEPADCCAQoCggEBAO3Ju83nWv97XXXAT3/f23Wwm2z7YVq9\r\nJJLOXsTUgnsQ7+JPPXpi/IVqrfDS8EIifbnjc5s8AI2sHUApurR1BLN0QBSnrXAr\r\nUGxGgyCuF7NjLnc9Q7f/kKhplokvFEqBv9DzsppDK67LCBnToEfkiTHkJgydYIfW\r\nApP42wMTDHizzWf3uZBOvF6QR6cKiQSLMUMQdIQ9HBl3XRJFIM/4iMpG2HmLLTJ+\r\nao/xWwlSpxThUuptvrad6qa5l6r8s14WYyfNq51G8Dq7NELymgrwgFiVH9GeB2XI\r\n/L2UxzsgtErLSdBuANZrl5V4QGrzuPN3QTcBmAe3ssUDgR3/7+yPl4UCAwEAAaNQ\r\nME4wDgYDVR0PAQH/BAQDAgWgMB0GA1UdJQQWMBQGCCsGAQUFBwMCBggrBgEFBQcD\r\nATAdBgNVHQ4EFgQUmo3NpTm88OuQdY/fyv9eVh1dJlAwDQYJKoZIhvcNAQEFBQAD\r\nggEBAA53S2/aPUeraUhxSiC8f8Nc3RZAaV4AcIJ2MkktTm64kKXHrGp/zt/D+zRf\r\nyTbe8XQAYxYOv8IyplRMBMPhGH7hjXIQPlTQX84lTA0odlGVUsIs57FB9JtRE0kQ\r\nRGqUzD/GGH8Uh8xxUvgiNuWAqWMktVLErYddQy41BiF2FU1qxZxc/I1Yr69XhYcV\r\nVRB9IS0mKT6dp1Zx2mP3krrhWq5dleuFu1dPTFQ6Ocy9fr1OXcYUgWvImz0BDn4l\r\nvOWQv+Py0QeIkHO9ZypYXwQBwOND5iGu8hElnM5mlWr5xVBFg1QFYnV5J7kpkWb4\r\nPixAHjDjcI+DCike5FYt5QVkXo0="
                        }
                    },
                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = { "https://localhost:7119/signin-codeflowprivatekeyjwt" },
                FrontChannelLogoutUri = "https://localhost:7119/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:7119/signout-callback-oidc" },

                AllowedScopes = { "openid", "profile", "email", "demoapi.fullaccess" }
            },
            // code flow + PKCE + JAR
            new Client
            {
                ClientId = "webclientdemojar",
                // ensure only signed requests are allowed
                RequireRequestObject = true,
                // the first secret is used for client auth
                // the second secret is used for JAR signature validation
                ClientSecrets = { 
                    new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()),
                    new Secret
                        {
                            Type = IdentityServerConstants.SecretTypes.X509CertificateBase64,
                            Value = "MIIDEDCCAfigAwIBAgIQFGwbjjyAs7FKzHhNUVfjbTANBgkqhkiG9w0BAQUFADAb\r\nMRkwFwYDVQQDDBBEZW1vIEpXVCBTaWduaW5nMB4XDTIzMDUwODA4NDYzN1oXDTI0\r\nMDUwODA5MDYzN1owGzEZMBcGA1UEAwwQRGVtbyBKV1QgU2lnbmluZzCCASIwDQYJ\r\nKoZIhvcNAQEBBQADggEPADCCAQoCggEBAO3Ju83nWv97XXXAT3/f23Wwm2z7YVq9\r\nJJLOXsTUgnsQ7+JPPXpi/IVqrfDS8EIifbnjc5s8AI2sHUApurR1BLN0QBSnrXAr\r\nUGxGgyCuF7NjLnc9Q7f/kKhplokvFEqBv9DzsppDK67LCBnToEfkiTHkJgydYIfW\r\nApP42wMTDHizzWf3uZBOvF6QR6cKiQSLMUMQdIQ9HBl3XRJFIM/4iMpG2HmLLTJ+\r\nao/xWwlSpxThUuptvrad6qa5l6r8s14WYyfNq51G8Dq7NELymgrwgFiVH9GeB2XI\r\n/L2UxzsgtErLSdBuANZrl5V4QGrzuPN3QTcBmAe3ssUDgR3/7+yPl4UCAwEAAaNQ\r\nME4wDgYDVR0PAQH/BAQDAgWgMB0GA1UdJQQWMBQGCCsGAQUFBwMCBggrBgEFBQcD\r\nATAdBgNVHQ4EFgQUmo3NpTm88OuQdY/fyv9eVh1dJlAwDQYJKoZIhvcNAQEFBQAD\r\nggEBAA53S2/aPUeraUhxSiC8f8Nc3RZAaV4AcIJ2MkktTm64kKXHrGp/zt/D+zRf\r\nyTbe8XQAYxYOv8IyplRMBMPhGH7hjXIQPlTQX84lTA0odlGVUsIs57FB9JtRE0kQ\r\nRGqUzD/GGH8Uh8xxUvgiNuWAqWMktVLErYddQy41BiF2FU1qxZxc/I1Yr69XhYcV\r\nVRB9IS0mKT6dp1Zx2mP3krrhWq5dleuFu1dPTFQ6Ocy9fr1OXcYUgWvImz0BDn4l\r\nvOWQv+Py0QeIkHO9ZypYXwQBwOND5iGu8hElnM5mlWr5xVBFg1QFYnV5J7kpkWb4\r\nPixAHjDjcI+DCike5FYt5QVkXo0="
                        }},

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = { "https://localhost:7119/signin-codeflowjar" },
                FrontChannelLogoutUri = "https://localhost:7119/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:7119/signout-callback-oidc" },

                AllowedScopes = { "openid", "profile", "email", "demoapi.fullaccess" }
            },
            // code flow + PKCE + private key JWT auth + JAR
            new Client
            {
                ClientId = "webclientdemojwtandjar",
                // ensure only signed requests are allowed
                RequireRequestObject = true,
                // this secret is used for both client auth (viat private key jwt)
                // and JAR signature validation
                ClientSecrets =
                    {
                        new Secret
                        {
                            Type = IdentityServerConstants.SecretTypes.X509CertificateBase64,
                            Value = "MIIDEDCCAfigAwIBAgIQFGwbjjyAs7FKzHhNUVfjbTANBgkqhkiG9w0BAQUFADAb\r\nMRkwFwYDVQQDDBBEZW1vIEpXVCBTaWduaW5nMB4XDTIzMDUwODA4NDYzN1oXDTI0\r\nMDUwODA5MDYzN1owGzEZMBcGA1UEAwwQRGVtbyBKV1QgU2lnbmluZzCCASIwDQYJ\r\nKoZIhvcNAQEBBQADggEPADCCAQoCggEBAO3Ju83nWv97XXXAT3/f23Wwm2z7YVq9\r\nJJLOXsTUgnsQ7+JPPXpi/IVqrfDS8EIifbnjc5s8AI2sHUApurR1BLN0QBSnrXAr\r\nUGxGgyCuF7NjLnc9Q7f/kKhplokvFEqBv9DzsppDK67LCBnToEfkiTHkJgydYIfW\r\nApP42wMTDHizzWf3uZBOvF6QR6cKiQSLMUMQdIQ9HBl3XRJFIM/4iMpG2HmLLTJ+\r\nao/xWwlSpxThUuptvrad6qa5l6r8s14WYyfNq51G8Dq7NELymgrwgFiVH9GeB2XI\r\n/L2UxzsgtErLSdBuANZrl5V4QGrzuPN3QTcBmAe3ssUDgR3/7+yPl4UCAwEAAaNQ\r\nME4wDgYDVR0PAQH/BAQDAgWgMB0GA1UdJQQWMBQGCCsGAQUFBwMCBggrBgEFBQcD\r\nATAdBgNVHQ4EFgQUmo3NpTm88OuQdY/fyv9eVh1dJlAwDQYJKoZIhvcNAQEFBQAD\r\nggEBAA53S2/aPUeraUhxSiC8f8Nc3RZAaV4AcIJ2MkktTm64kKXHrGp/zt/D+zRf\r\nyTbe8XQAYxYOv8IyplRMBMPhGH7hjXIQPlTQX84lTA0odlGVUsIs57FB9JtRE0kQ\r\nRGqUzD/GGH8Uh8xxUvgiNuWAqWMktVLErYddQy41BiF2FU1qxZxc/I1Yr69XhYcV\r\nVRB9IS0mKT6dp1Zx2mP3krrhWq5dleuFu1dPTFQ6Ocy9fr1OXcYUgWvImz0BDn4l\r\nvOWQv+Py0QeIkHO9ZypYXwQBwOND5iGu8hElnM5mlWr5xVBFg1QFYnV5J7kpkWb4\r\nPixAHjDjcI+DCike5FYt5QVkXo0="
                        }
                    },
                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = { "https://localhost:7119/signin-codeflowprivatekeyjwtandjar" },
                FrontChannelLogoutUri = "https://localhost:7119/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:7119/signout-callback-oidc" },

                AllowedScopes = { "openid", "profile", "email", "demoapi.fullaccess" }
            },
            // code flow + PKCE + token encryption
            new Client
            {
                ClientId = "webclientdemotokenencryption",
                ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = { "https://localhost:7119/signin-codeflowtokenencryption" },
                FrontChannelLogoutUri = "https://localhost:7119/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:7119/signout-callback-oidc" },

                AllowedScopes = { "openid", "profile", "email", "demoapi.fullaccess" }
            },
            // client credentials, DPoP
            new Client
            {
                ClientId = "dpopclient", 
                RequireDPoP = true,
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = { "demoapi.fullaccess" }, 
                ClientSecrets = { 
                    new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },
            },
             // code flow + PKCE + DPoP
            new Client
            {
                ClientId = "webclientdemodpop",
                ClientSecrets = { new Secret("49C1A7E1-0C79-4A89-A3D6-A37998FB86B0".Sha256()) },
                RequireDPoP = true,
                AllowedGrantTypes = GrantTypes.Code,
                RedirectUris = { "https://localhost:7119/signin-codeflowdpop" },
                FrontChannelLogoutUri = "https://localhost:7119/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:7119/signout-callback-oidc" },
                AllowedScopes = { "openid", "profile", "email", "demoapi.fullaccess" }
            },
        };
}
