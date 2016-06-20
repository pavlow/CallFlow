using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;
using CallFlowManager.UI.ViewModels.BusinessHours;
using CallFlowManager.UI.ViewModels.Groups;
using CallFlowManager.UI.ViewModels.Queues;

namespace CallFlowManager.UI.Business
{
    class QueuesSetService
    {
        /// <summary> 
        /// Prepares set command by creating a PS object that will be passed to the script file
        /// http://stackoverflow.com/questions/12472718/pass-collection-of-c-sharp-psobjects-to-powershell-pipeline
        /// <param name="description"></param>
        /// <param name="group"></param>
        /// <param name="groups"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="overFlow"></param>
        /// <param name="overFlowDestination"></param>
        /// <param name="overFlowSipDomain"></param>
        /// <param name="overFlowUri"></param>
        /// <param name="overflowOn"></param>
        /// <param name="timeout"></param>
        /// <param name="timeoutDestination"></param>
        /// <param name="timeoutOn"></param>
        /// <param name="timeoutSipDomain"></param>
        /// <param name="timeoutUri"></param>
        /// <returns></returns>
        public PSObject PrepareSetCsRgsQueue(string description, string group, ObservableCollection<QueueGroupViewModel>groups, string id, string name, 
                        int overFlow, string overFlowDestination, string overFlowSipDomain, string overFlowUri, bool overflowOn, string overflowCandidate,
                        QueueViewModel overflowQueue, int timeout, string timeoutDestination, bool timeoutOn, string timeoutSipDomain, string timeoutUri, QueueViewModel timeoutQueue, string ownerPool)
        {
            //Create agents PS object
            Collection<PSObject> groupsObj = new Collection<PSObject>();
            foreach (var item in groups)
            {
                PSObject obj1 = new PSObject();
                obj1.Properties.Add(new PSNoteProperty("QueueGroups", item.Name));
                groupsObj.Add(obj1);
            }

            //Create PS object and return it
            PSObject queuesObj = new PSObject();
            queuesObj.Properties.Add(new PSNoteProperty("AgentGroupIDList", groupsObj));
            queuesObj.Properties.Add(new PSNoteProperty("Name", name));
            queuesObj.Properties.Add(new PSNoteProperty("OwnerPool", ownerPool));
            queuesObj.Properties.Add(new PSNoteProperty("Identity", id));
            queuesObj.Properties.Add(new PSNoteProperty("Description", description));

            //Overflow
            queuesObj.Properties.Add(new PSNoteProperty("OverflowOn", overflowOn));
            queuesObj.Properties.Add(new PSNoteProperty("OverflowThreshold", overFlow));
            queuesObj.Properties.Add(new PSNoteProperty("OverflowDestination", overFlowDestination));
            queuesObj.Properties.Add(new PSNoteProperty("OverflowCandidate", overflowCandidate));
            queuesObj.Properties.Add(new PSNoteProperty("OverflowUri", overFlowUri));
            queuesObj.Properties.Add(new PSNoteProperty("OverflowSipDomain", overFlowSipDomain));
            queuesObj.Properties.Add(new PSNoteProperty("OverflowQueue", overflowQueue));

            //Timeout
            queuesObj.Properties.Add(new PSNoteProperty("TimeoutOn", timeoutOn));
            queuesObj.Properties.Add(new PSNoteProperty("TimeoutThreshold", timeout));
            queuesObj.Properties.Add(new PSNoteProperty("TimeoutDestination", timeoutDestination));
            queuesObj.Properties.Add(new PSNoteProperty("TimeoutUri", timeoutUri));
            queuesObj.Properties.Add(new PSNoteProperty("TimeoutSipDomain", timeoutSipDomain));
            queuesObj.Properties.Add(new PSNoteProperty("TimeoutQueue", timeoutQueue));
            
            return queuesObj;
        }
    }
}
