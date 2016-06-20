using System.Management.Automation;
using System.Configuration;
using System;
using System.Management.Automation.Runspaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Lync_WCF
{
    public class UmUserManager
    {
        private readonly string _identity;
        private readonly string _domainController;
        private readonly string _connectionUri;
        private PsFactory _ps;

        public UmUserManager(string identity, string domainController, string connectionUri)
        {
            _identity = identity;
            _domainController = domainController;
            _connectionUri = connectionUri;
            _ps = new PsFactory(connectionUri, Constrants.ExchangeShellUri);
        }

        public void Enable(string mailboxPolicy, string extensions, string pin, string sipAddress)
        {
            if (!string.IsNullOrWhiteSpace(sipAddress) && sipAddress.StartsWith("sip:", StringComparison.InvariantCultureIgnoreCase))
            {
                sipAddress = sipAddress.Substring("sip:".Length, sipAddress.Length - "sip:".Length);
            }
            if (string.IsNullOrWhiteSpace(pin)) pin = "$null";

            var parameters = new Dictionary<string, object>();
            parameters["identity"] = _identity;
            parameters["mailboxPolicy"] = mailboxPolicy;
            parameters["extensions"] = extensions;
            parameters["pin"] = pin;
            parameters["sipAddress"] = sipAddress;
            parameters["domainController"] = _domainController;
            _ps.RunPowerShellScriptFile("EnableUmMailboxRemoting.ps1", parameters);
        }

        public void Disable()
        {
            var parameters = new Dictionary<string, object>();
            parameters["identity"] = _identity;
            parameters["domainController"] = _domainController;
            _ps.RunPowerShellScriptFile("DisableUmMailboxRemoting.ps1", parameters);
        }

        public void ResetPin(string pin)
        {
            var parameters = new Dictionary<string, object>();
            parameters["identity"] = _identity;
            parameters["pin"] = pin;
            parameters["domainController"] = _domainController;
            _ps.RunPowerShellScriptFile("SetUmMailboxPinRemoting.ps1", parameters);
        }

    }
}