using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Security;
using EPiServer.Web.Routing;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.WsFederation;
using SSOPOC.Helpers;
using SSOPOC.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SSOPOC.Controllers
{
    public class LoginController : Controller
    {
        private UIUserProvider UIUserProvider => ServiceLocator.Current.GetInstance<UIUserProvider>();
        private UIUserManager UIUserManager => ServiceLocator.Current.GetInstance<UIUserManager>();
        private UISignInManager UISignInManager => ServiceLocator.Current.GetInstance<UISignInManager>();

        public ActionResult Index()
        {
            return View("/Views/Login/Index.cshtml");
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Index(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var resFromSignIn = UISignInManager.SignIn(UIUserProvider.Name, model.Username, model.Password);
                if (resFromSignIn)
                {
                    return Redirect(UrlResolver.Current.GetUrl("episerver/cms"));
                }
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("LoginError", "Login failed");
            return View("/Views/Login/Index.cshtml", model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult AdfsLogin(string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var properties = new AuthenticationProperties { RedirectUri = returnUrl };
                HttpContext.GetOwinContext().Authentication.Challenge(properties, WsFederationAuthenticationDefaults.AuthenticationType);
            }
            return View("/Views/Login/Index.cshtml");
        }
    }
}