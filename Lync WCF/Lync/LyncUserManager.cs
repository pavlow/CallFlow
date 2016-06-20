using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lync_WCF
{
    public class LyncUserManager
    {
        private readonly PsFactory _factory = new LyncPsFactory();
        private string _identity;
        private string _domainController;

        public LyncUserManager(string identity)
        {
            _identity = identity;
        }

        public LyncUserManager(string identity, string domainController)
        {
            _identity = identity;
            _domainController = domainController;
        }

        public void Enable(string registrarPool, string sipAddress)
        {
            var parameters = new Dictionary<string, object>();
            parameters["registrarPool"] = registrarPool;
            parameters["identity"] = _identity;
            parameters["domainController"] = _domainController;
            parameters["sipAddress"] = sipAddress;
            _factory.RunPowerShellScriptFile("EnableCsUser.ps1", parameters);
        }

        public void Disable()
        {
            var parameters = new Dictionary<string, object>();
            parameters["identity"] = _identity;
            parameters["domainController"] = _domainController;
            _factory.RunPowerShellScriptFile("DisableCsUser.ps1", parameters);
        }

        public void SetUserGroupingId(string customerOuPath, string primaryDomain)
        {
            var parameters = new Dictionary<string, object>();
            parameters["customerOUPath"] = customerOuPath;
            parameters["userName"] = _identity;
            parameters["primaryDomain"] = primaryDomain;
            _factory.RunPowerShellScriptFile("SetUserGroupingID.ps1", parameters);
        }

        public void ClearUserGroupingId(string customerOuPath, string primaryDomain)
        {
            var parameters = new Dictionary<string, object>();
            parameters["customerOuPath"] = customerOuPath;
            parameters["userName"] = _identity;
            parameters["primaryDomain"] = primaryDomain;
            _factory.RunPowerShellScriptFile("ClearUserGroupingID.ps1", parameters);
        }

        public void GrantPolicies(string dialplan, string voice, string conferencing)
        {
            var parameters = new Dictionary<string, object>();
            parameters["identity"] = _identity;
            parameters["dialplan"] = dialplan;
            parameters["voice"] = voice;
            parameters["conferencing"] = conferencing;
            parameters["domainController"] = _domainController;
            _factory.RunPowerShellScriptFile("GrantCsUserPolicies.ps1", parameters);
        }

        public void SetUserTelephonyOption(TelephonyOption option, string lineUri, string lineServerUri)
        {
            var parameters = new Dictionary<string, object>();
            parameters["identity"] = _identity;
            parameters["telephonyOption"] = option.ToString();
            parameters["lineUri"] = lineUri;
            parameters["lineServerUri"] = lineServerUri;
            parameters["domainController"] = _domainController;
            _factory.RunPowerShellScriptFile("SetTelephonyOption.ps1", parameters);
        }
    }
}