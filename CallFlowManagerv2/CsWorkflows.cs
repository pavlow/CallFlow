using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace CallFlowManagerv2
{
    class CsWorkflows
    {
        //Get Sip Domains
        public Collection<PSObject> GetSipDomains()
        {
            var psCommand = @"Get-CsSipDomain | Select Name";
            PsExecutor pS = new PsExecutor();
            var pSSync = pS.ExecuteSynchronously(psCommand);

            return pSSync;

        }

    }
}
