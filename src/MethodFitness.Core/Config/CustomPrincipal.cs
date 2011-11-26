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
        private string _tenantId;
        private string _loginInfoId;

        public CustomPrincipal(IIdentity identity, string userData)
        {
            _identity = identity;
            var data = userData.Split('|');
            var userIdProp = data.FirstOrDefault(x=>x.Contains("UserId="));
            _userId = userIdProp.Replace("UserId=","");
            var orgIdProp = data.FirstOrDefault(x => x.Contains("OrgId="));
            _orgId = orgIdProp.Replace("OrgId=", "");
            var tenantIdProp = data.FirstOrDefault(x => x.Contains("TenantId="));
            _tenantId = tenantIdProp.Replace("TenantId=", "");
            var loginInfoIdProp = data.FirstOrDefault(x => x.Contains("LoginInfoId="));
            _loginInfoId = loginInfoIdProp.Replace("LoginInfoId=", "");
        }

        public bool IsInRole(string role)
        {
            return false;
        }

        public IIdentity Identity{ get { return _identity; } }

        public int UserId { get{return _userId.IsNotEmpty()? Int32.Parse(_userId):0;} }
        public int TenantId { get { return _tenantId.IsNotEmpty() ? Int32.Parse(_tenantId) : 0; } }
        public int OrgId { get { return _orgId.IsNotEmpty() ? Int32.Parse(_orgId) : 0; } }
        public int LoginInfoId { get { return _loginInfoId.IsNotEmpty() ? Int32.Parse(_loginInfoId) : 0; } }
    }
}