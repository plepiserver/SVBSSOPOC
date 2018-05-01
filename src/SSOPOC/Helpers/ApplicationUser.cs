using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace SSOPOC.Helpers
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsApproved { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime CreationDate { get; set; }

        public ApplicationUser()
        {
        }

        public ApplicationUser(string username) : base(username)
        {

        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}