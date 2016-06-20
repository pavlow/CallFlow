using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace CallFlowManager.UI.Business
{
    class SharedPsService
    {
        public List<string> ProcessRegistrarPools(ICollection<PSObject> psObject)
        {
            List<string> Pools = new List<string>();

            if (psObject != null)
            {
                foreach (dynamic registrarPools in psObject)
                {
                    if (registrarPools != null)
                    {
                        Pools.Add(registrarPools.Fqdn);
                    }
                }
            }

            return Pools;
        }
        
    }
}
