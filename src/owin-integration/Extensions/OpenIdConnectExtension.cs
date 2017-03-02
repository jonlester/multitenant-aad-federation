using Microsoft.IdentityModel.Protocols;
using Microsoft.Owin.Security.OpenIdConnect;
using multitenant_aad_federation.owin_integration.Configuration;
using multitenant_aad_federation.owin_integration.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Owin
{
    public static class OpenIdConnectExtension
    {
        public static IAppBuilder UseMultitenantAadOidcAuthentication(this IAppBuilder app, MultitenantAadOidcOptions options)
        {
            if (app == null)
                throw new ArgumentNullException("app");
            if (options == null)
                throw new ArgumentNullException("options");

            var aad = new OpenIdConnectAuthenticationOptions
            {
                AuthenticationType = options.AuthenticationType,
                Caption = options.Caption,
                RedirectUri = options.RedirectUri,
                ClientId = options.MultitenantAppId.ToString(),
                Authority = options.Authority,
                Scope = "openid email",
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    RedirectToIdentityProvider = (n) =>
                    {
                        var metadata = MetadataHelper.GetMetadataDocumentAsync("https://login.microsoftonline.com/lesterjtgmail.onmicrosoft.com/.well-known/openid-configuration", n.OwinContext.Request.CallCancelled).Result;
                        OpenIdConnectConfiguration config = new OpenIdConnectConfiguration(metadata);

                        //these values affect the redirect
                        n.ProtocolMessage.IssuerAddress = config.AuthorizationEndpoint;
                        n.ProtocolMessage.ClientId = "4deaba2a-548d-47bc-809f-e010a0841769";

                        //these values affect the token validation when the reponse is received
                        n.Options.TokenValidationParameters.ValidAudience = "4deaba2a-548d-47bc-809f-e010a0841769";
                        n.Options.TokenValidationParameters.ValidIssuer = config.Issuer;

                        return Task.FromResult(0);
                    }
                },
                SignInAsAuthenticationType = options.SignInAsAuthenticationType
            };
            app.UseOpenIdConnectAuthentication(aad);

            return app;
        }
    }
}
