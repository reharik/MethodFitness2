using System;
using System.Linq;
using System.Security.Principal;

namespace MethodFitness.Core.Config
{
    public class CustomPrincipal : IPrincipal
    {
        private readonly IIdentity _identity;
        private string _userId;
        private string _orgId;

        public CustomPrincipal(IIdentity identity, string userData)
        {
            _identity = identity;
            var data = userData.Split('|');
            var userIdProp = data.FirstOrDefault(x=>x.Contains("UserId="));
            _userId = userIdProp.Replace("UserId=","");
            var orgIdProp = data.FirstOrDefault(x => x.Contains("OrgId="));
            _orgId = orgIdProp.Replace("OrgId=", "");
        }

        public bool IsInRole(string role)
        {
            return false;
        }

        public IIdentity Identity{ get { return _identity; } }

        public int UserId { get{return _userId.IsNotEmpty()? Int32.Parse(_userId):0;} }
        public int OrgId { get { return _orgId.IsNotEmpty() ? Int32.Parse(_orgId) : 0; } }
    }
}