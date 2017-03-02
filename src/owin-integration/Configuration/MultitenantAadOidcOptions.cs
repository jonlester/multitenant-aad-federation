using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace multitenant_aad_federation.owin_integration.Configuration
{
    public class MultitenantAadOidcOptions
    {
        public MultitenantAadOidcOptions()
        {
            AuthenticationType = "MultitenantAAD";
            Caption = "Azure AD";
            Authority = "https://login.microsoftonline.com/common";
        
        }
        public string AuthenticationType { get; set; }
        public string Caption { get; set; }

        public string Authority { get; set; }
        /// <summary>
        /// Used for "client_id" when authenticating to an AAD common endpoint.  
        /// App should exist in your own AAD tenant, and be configured as multi-tenant
        /// Value is not required for federation with specific known tenants
        /// </summary>
        public Guid MultitenantAppId { get; set; }

        /// <summary>
        /// Not required, but optional if a subpath is desired.  The left-hand side of the Url must
        /// match the Reply Url configured for this application in Azure AD or the auth endpoint 
        /// not all the user to log in
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1056:UriPropertiesShouldNotBeStrings", Justification = "By Design")]
        public string RedirectUri { get; set; }
        public string SignInAsAuthenticationType { get; set; }
    }
}
