using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// The following using statements were added for this sample
using Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Notifications;
using Microsoft.IdentityModel.Protocols;
using System.Web.Mvc;
using System.Configuration;
using System.IdentityModel.Tokens;
using WebApp_B2C_DotNet.App_Start;

namespace WebApp_B2C_DotNet
{
	public partial class Startup
	{
        private const string discoverySuffix = "/.well-known/openid-configuration";
        public const string AcrClaimType = "http://schemas.microsoft.com/claims/authnclassreference";

        // App config settings
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AadInstance"];
        private static string tenant = ConfigurationManager.AppSettings["ida:Tenant"];
        private static string redirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"];

        // B2C policy identifiers
        public static string SignUpPolicyId = ConfigurationManager.AppSettings["ida:SignUpPolicyId"];
        public static string SignInPolicyId = ConfigurationManager.AppSettings["ida:SignInPolicyId"];
        //public static string ResetPolicyId = ConfigurationManager.AppSettings["ida:PasswordResetPolicyId"];
        public static string ProfilePolicyId = ConfigurationManager.AppSettings["ida:UserProfilePolicyId"];

        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            OpenIdConnectAuthenticationOptions b2coptions = new OpenIdConnectAuthenticationOptions
            {
                // Standard OWIN OIDC parameters
                Authority = String.Format(aadInstance, tenant),
                ClientId = clientId,
                RedirectUri = redirectUri,
                PostLogoutRedirectUri = redirectUri,
                Notifications = new OpenIdConnectAuthenticationNotifications
                { 
                    AuthenticationFailed = AuthenticationFailed,
                },

                // Required for AAD B2C
                Scope = "openid",
                ConfigurationManager = new B2CConfigurationManager(String.Format(aadInstance + "{1}", tenant, discoverySuffix)),

                // Optional - used for displaying the user's name in the navigation bar when signed in.
                TokenValidationParameters = new TokenValidationParameters
                {  
                    NameClaimType = "name",
                },

                AuthenticationType = "OpenIdConnect-B2C",
            };

            // Required for AAD B2C
            app.Use(typeof(B2COpenIdConnectAuthenticationMiddleware), app, b2coptions);

            OpenIdConnectAuthenticationOptions b2eoptions = new OpenIdConnectAuthenticationOptions
            {
                Authority = String.Format(aadInstance, "common"),
                ClientId = clientId,
                RedirectUri = redirectUri,
                PostLogoutRedirectUri = redirectUri,
                Notifications = new OpenIdConnectAuthenticationNotifications
                {
                    AuthenticationFailed = AuthenticationFailed,
                },

                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                },

                AuthenticationType = "OpenIdConnect-B2E",
            };

            app.UseOpenIdConnectAuthentication(b2eoptions);
        }

        // Used for avoiding yellow-screen-of-death
        private Task AuthenticationFailed(AuthenticationFailedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> notification)
        {
            notification.HandleResponse();
            notification.Response.Redirect("/Home/Error?message=" + notification.Exception.Message);
            return Task.FromResult(0);
        }
    }
}