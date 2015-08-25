using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

// The following using statements were added for this sample.
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.Owin.Security.Cookies;
using WebApp_B2C_DotNet.App_Start;
using System.Security.Claims;

namespace WebApp_B2C_DotNet.Controllers
{
    public class AccountController : Controller
    {
        public void SignInB2C()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties(
                        new Dictionary<string, string> 
                        { 
                            {B2COpenIdConnectAuthenticationHandler.PolicyParameter, Startup.SignInPolicyId} 
                        })
                    {
                        RedirectUri = "/",
                    },
                    "OpenIdConnect-B2C");
            }
        }

        public void SignInWork()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties
                    {
                        RedirectUri = "/",
                    },
                    "OpenIdConnect-B2E");
            }
        }

        public void SignUpB2C()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties(
                        new Dictionary<string, string> 
                        { 
                            {B2COpenIdConnectAuthenticationHandler.PolicyParameter, Startup.SignUpPolicyId} 
                        })
                    {
                        RedirectUri = "/",
                    },
                    "OpenIdConnect-B2C");
            }
        }


        public void Profile()
        {
            if (Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(
                    new AuthenticationProperties(
                        new Dictionary<string, string> 
                        { 
                            {B2COpenIdConnectAuthenticationHandler.PolicyParameter, Startup.ProfilePolicyId} 
                        })
                    {
                        RedirectUri = "/",
                    },
                    "OpenIdConnect-B2C");
            }
        }

        // Password Reset in Progress
        //public void PasswordReset()
        //{
        //    if (Request.IsAuthenticated)
        //    {
        //        HttpContext.GetOwinContext().Authentication.Challenge(
        //            new AuthenticationProperties(
        //                new Dictionary<string, string> 
        //                { 
        //                    {B2COpenIdConnectAuthenticationHandler.PolicyParameter, Startup.ResetPolicyId} 
        //                })
        //            {
        //                RedirectUri = "/",
        //            },
        //            OpenIdConnectAuthenticationDefaults.AuthenticationType);
        //    }
        //}

        public void SignOut()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            if (ClaimsPrincipal.Current.FindFirst(Startup.AcrClaimType) != null)
            {
                dict[B2COpenIdConnectAuthenticationHandler.PolicyParameter] = ClaimsPrincipal.Current.FindFirst(Startup.AcrClaimType).Value;
                HttpContext.GetOwinContext().Authentication.SignOut(
                    new AuthenticationProperties(dict),
                    "OpenIdConnect-B2C", CookieAuthenticationDefaults.AuthenticationType);
            }
            else
            {
                HttpContext.GetOwinContext().Authentication.SignOut(
                    new AuthenticationProperties(dict),
                    "OpenIdConnect-B2E", CookieAuthenticationDefaults.AuthenticationType);
            }
            
            
        }
	}
}