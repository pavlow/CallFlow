using System.Management.Automation;
using System.Configuration;
using System;
using System.Management.Automation.Runspaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Lync_WCF
{
    public class UmServerManager
    {
        private string _domainController;
        private readonly string _connectionUri;
        private PsFactory _ps;

        public UmServerManager(string domainController, string connectionUri)
        {
            _domainController = domainController;
            _connectionUri = connectionUri;
            _ps = new PsFactory(connectionUri, Constrants.ExchangeShellUri);
        }

        public void AddDialPlan(string dialPlanName, string regionCode, string accessNumber)
        {
            var parameters = new Dictionary<string, object>();
            parameters["dialPlanName"] = dialPlanName;
            parameters["regionCode"] = regionCode;
            parameters["accessNumber"] = accessNumber;
            parameters["domainController"] = _domainController;
            _ps.RunPowerShellScriptFile("AddUmDialPlanRemoting.ps1", parameters);
        }

        public void RemoveDialPlan(string dialPlanName)
        {
            var parameters = new Dictionary<string, object>();
            parameters["dialPlanName"] = dialPlanName;
            parameters["domainController"] = _domainController;
            try
            {
                _ps.RunPowerShellScriptFile("RemoveUmDialPlanRemoting.ps1", parameters);
            }
            catch (Exception ex)
            {
                // Not ideal to look for error messages...
                // The cmdlet throws an exception if we try to remove a dial plan even though it doesn't exist
                if (ex.Message.IndexOf("doesn't exist", StringComparison.InvariantCultureIgnoreCase) == -1 
                    || (ex.InnerException != null && ex.InnerException.Message.IndexOf("doesn't exist", StringComparison.InvariantCultureIgnoreCase) == -1))
                {
                    throw;
                }
            }
        }

    }
}