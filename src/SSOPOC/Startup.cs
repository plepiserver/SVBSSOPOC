using System;
using System.Configuration;
using System.IdentityModel.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.WsFederation;
using Owin;
using Sustainsys.Saml2.Owin;

[assembly: OwinStartup(typeof(SSOPOC.Startup))]

namespace SSOPOC
{
    public class Startup
    {
        const string LogoutUrl = "/util/logout.aspx";

        public void Configuration(IAppBuilder app)
        {
            // Add CMS integration for ASP.NET Identity
            app.AddCmsAspNetIdentity<ApplicationUser>();

            app.SetDefaultSignInAsAuthenticationType(WsFederationAuthenticationDefaults.AuthenticationType);

            // local Episerver authentication
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager<ApplicationUser>, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => manager.GenerateUserIdentityAsync(user))
                }
            });

            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.UseSaml2Authentication(new Saml2AuthenticationOptions(true));

            //Tell antiforgery to use the name claim
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.Name;
        }
    }
}
