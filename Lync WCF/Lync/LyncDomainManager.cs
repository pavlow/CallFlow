using System;
using System.Collections.Generic;

namespace Lync_WCF
{
    public class LyncDomainManager
    {
        private readonly PsFactory _factory = new LyncPsFactory();

        public void Add(string domain)
        {
            try
            {
                var parameters = new Dictionary<string, object>();
                parameters["sipDomain"] = domain;
                _factory.RunPowerShellScriptFile("AddSipDomain.ps1", parameters);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("Failed to add the SIP domain {0}. Error: {1}", domain, ex.Message), ex);
            }
        }

        public void Remove(string domain)
        {
            try
            {
                var parameters = new Dictionary<string, object>();
                parameters["sipDomain"] = domain;
                _factory.RunPowerShellScriptFile("RemoveSipDomain.ps1", parameters);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(string.Format("Failed to remove the SIP domain {0}. Error: {1}", domain, ex.Message), ex);
            }
        }
    }
}