using EPiServer.Async;
using EPiServer.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace SSOPOC.Helpers
{
    public class PingFedSynchronizingUserService : ISynchronizingUserService
    {
        private readonly TaskExecutor _taskExecutor;

        public PingFedSynchronizingUserService(TaskExecutor taskExecutor)
        {
            _taskExecutor = taskExecutor;
        }

        public virtual Task SynchronizeAsync(ClaimsIdentity identity, IEnumerable<string> additionalClaimsToSync) =>
            _taskExecutor.Start(new Action(delegate
            {
                this.SynchronizeUserAndClaims(identity, additionalClaimsToSync);
            }));

        internal virtual void SynchronizeUserAndClaims(ClaimsIdentity identity, IEnumerable<string> additionalClaimsToSync)
        {
            foreach (var claim in identity.Claims)
            {
                if (claim.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/WS-Federation")
                {
                    List<string> claimValues = new List<string>(claim.Value.Split(','));
                    foreach (var claimValue in claimValues)
                    {
                        if (claimValue.StartsWith("CN="))
                        {
                            identity.AddClaim(new Claim(ClaimTypes.Role, claimValue.Replace("CN=", string.Empty)));
                        }
                    }
                }
                if (claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")
                {
                    identity.AddClaim(new Claim(ClaimTypes.UserData, claim.Value));
                    identity.AddClaim(new Claim(ClaimTypes.Name, claim.Value));
                }

                // Lookup users here
                // Create user and assign roles here
            }
        }
    }
}