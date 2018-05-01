using EPiServer.ServiceLocation;
using EPiServer.Shell.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using SSOPOC.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SSOPOC.Controllers
{
    public class PingOneController : Controller
    {
        private UIUserProvider _UIUserProvider => ServiceLocator.Current.GetInstance<UIUserProvider>();
        private UISignInManager _UISignInManager => ServiceLocator.Current.GetInstance<UISignInManager>();
        private UIRoleProvider _UIRoleProvider => ServiceLocator.Current.GetInstance<UIRoleProvider>();

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
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult SSOLogin(string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult("Saml2", Url.Action("AuthenticateCallBack", "PingOne", new { ReturnUrl = returnUrl }));
        }

        public async Task<ActionResult> AuthenticateCallBack()
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            // Sign in the user with this external login provider if the user already has a login

            //var list = ApplicationDbContext.Roles.OrderBy(r => r.Name).ToList().Select(rr => new SelectListItem { Value = rr.Name.ToString(), Text = rr.Name }).ToList();
            // Add role if not exist
            //ApplicationDbContext.Roles.Add(new IdentityRole()
            //{
            //    Name = collection["RoleName"]
            //});
            //ApplicationDbContext.SaveChanges();
            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = "tester3",
                    Email = "tester3@abc.com",
                    IsApproved = true,
                    IsLockedOut = false,
                    CreationDate = DateTime.Now
                };
                var errors = Enumerable.Empty<string>();

                var result = await UserManager.CreateAsync(user, "Amer1canDre@m!");
                if (result.Succeeded)
                {
                    await UserManager.AddToRoleAsync(user.Id, "WebAdmins");
                    result = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
                    }
                    return Redirect("/episerver/cms");
                }
                else
                {
                    var result1 = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
                    switch (result1)
                    {
                        case SignInStatus.Success:
                            return Redirect("/episerver/cms");
                        case SignInStatus.LockedOut:
                            return Redirect("/episerver/cms");
                        case SignInStatus.RequiresVerification:
                            return Redirect("/episerver/cms");
                        case SignInStatus.Failure:
                            return Redirect("/episerver/cms");
                        default:
                            return Redirect("/episerver/cms");
                    }
                    return Redirect("/episerver/cms");
                }


                return Redirect("/episerver/cms");
            }

            return Redirect("/episerver/cms");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            private const string XsrfKey = "XsrfId";

            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
    }
}