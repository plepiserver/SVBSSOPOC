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
using System.Web.Security;

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
            var userData = RetreiveUserDataFromClaims(loginInfo);
            var errors = new List<string>();

            // Save roles first
            if (userData?.Roles?.Count > 0)
                SaveRoles(userData.Roles);

            var user = await UserManager.FindAsync(loginInfo.Login);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = userData.Username,
                    Email = $"{Guid.NewGuid()}@episerver.com",
                    IsApproved = true,
                    IsLockedOut = false,
                    CreationDate = DateTime.Now
                };

                //var createUserResult = await UserManager.CreateAsync(user, Membership.GeneratePassword(12, 6));
                var createUserResult = await UserManager.CreateAsync(user, "Amer1canDre@m!");
                if (createUserResult.Succeeded)
                {
                    foreach(var role in userData.Roles)
                    {
                        await UserManager.AddToRoleAsync(user.Id, role);
                    }
                    var result = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
                    if (!result.Succeeded)
                    {
                        foreach(var error in result.Errors)
                        {
                            errors.Add(error);
                        }
                    }
                }
                else
                {
                    foreach (var error in createUserResult.Errors)
                    {
                        errors.Add(error);
                    }
                }
            }

            if(errors.Count == 0)
            {
                await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
                return View("Success", userData);
            }
            else
            {
                ViewBag.Errors = new List<string>();
                foreach(var error in errors)
                {
                    ViewBag.Erros.Add(error);
                }
                return View("Error");
            }
        }

        private void SaveRoles(List<string> roles)
        {
            var list = ApplicationDbContext.Roles.OrderBy(r => r.Name).ToList();
            foreach (var role in roles)
            {
                if (list.Any(x => x.Name.Equals(role, StringComparison.InvariantCultureIgnoreCase)))
                    continue;
                else
                {
                    ApplicationDbContext.Roles.Add(new IdentityRole()
                    {
                        Name = role
                    });
                }
            }
            ApplicationDbContext.SaveChanges();
        }

        private ClaimUserData RetreiveUserDataFromClaims(ExternalLoginInfo loginInfo)
        {
            var rolePrefix = "DELG-Episerver-";
            var userData = new ClaimUserData();
            userData.Username = loginInfo.Login.ProviderKey;
            userData.Roles = new List<string>();
            foreach (var claim in loginInfo.ExternalIdentity?.Claims)
            {
                if (claim.Type.ToLower() == "roles")
                {
                    var roleName = claim.Value.Remove(0, rolePrefix.Count());
                    userData.Roles.Add(roleName);
                }
            }
            return userData;
        }
    }
}