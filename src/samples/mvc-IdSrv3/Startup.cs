using IdentityModel.Client;
using IdentityServer3.Core;
using IdentityServer3.Core.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using multitenant_aad_federation.owin_integration.Configuration;
using mvc_IdSrv3.IdentityServer;
using Owin;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Helpers;

[assembly: OwinStartupAttribute(typeof(mvc_IdSrv3.Startup))]
namespace mvc_IdSrv3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = Constants.ClaimTypes.Subject;
            JwtSecurityTokenHandler.InboundClaimTypeMap = new Dictionary<string, string>();

            app.Map("/identity", idsrvApp =>
            {
                idsrvApp.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = "Embedded IdentityServer",
                    SigningCertificate = LoadCertificate(),

                    Factory = new IdentityServerServiceFactory()
                                .UseInMemoryUsers(Users.Get())
                                .UseInMemoryClients(Clients.Get())
                                .UseInMemoryScopes(Scopes.Get()),

                    AuthenticationOptions = new IdentityServer3.Core.Configuration.AuthenticationOptions
                    {
                        EnablePostSignOutAutoRedirect = true,
                        IdentityProviders = ConfigureAzureAD
                    }
                });
            });

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "Cookies"
            });


            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                Authority = "https://localhost:44373/identity",
                ClientId = "mvc",
                Scope = "openid profile roles",
                ResponseType = "id_token token",
                RedirectUri = "https://localhost:44373/",
                SignInAsAuthenticationType = "Cookies",
                UseTokenLifetime = false
            });
        }

        private void ConfigureAzureAD(IAppBuilder app, string signInAsType)
        {
            var options = new MultitenantAadOidcOptions
            {
                AuthenticationType = "MultitenantAzureAd",
                Caption = "Azure AD",
                SignInAsAuthenticationType = signInAsType//,
               // RedirectUri = "https://localhost:1847/identity/"
            };
            app.UseMultitenantAadOidcAuthentication(options); 
        }
        
        X509Certificate2 LoadCertificate()
        {
            return new X509Certificate2(
                string.Format(@"{0}\bin\identityServer\idsrv3test.pfx", AppDomain.CurrentDomain.BaseDirectory), "idsrv3test");
        }
    }
}
