using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SSOPOC.Helpers
{
    public class ClaimUserData
    {
        public string ProviderKey { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public List<string> Roles { get; set; }
    }
}