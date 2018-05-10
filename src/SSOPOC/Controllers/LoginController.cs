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

        private ApplicationDbContext _context;
        public ApplicationDbContext ApplicationDbContext
        {
            get
            {
                return _context ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            }
            private set { _context = value; }
        }

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        private ApplicationSignInManager _signInManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set { _signInManager = value; }
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ActionResult Index()
        {
            var context = ControllerContext.HttpContext;
            if (context.User != null && context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                return new RedirectResult("Login/UnAuthorized");
            }
            return View("/Views/Login/Index.cshtml");
        }

        public ActionResult UnAuthorized()
        {
            return View("/Views/Login/UnAuthorized.cshtml");
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
                    return Redirect("/episerver/cms");
                }
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("LoginError", "Login failed");
            return View("/Views/Login/Index.cshtml", model);
        }
    }
}