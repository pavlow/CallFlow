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

namespace CallFlowManager.UI.Business
{
    class GroupsSetService
    {
        /// <summary>
        /// Prepares set command by creating a PS object that will be passed to the script file
        /// http://stackoverflow.com/questions/12472718/pass-collection-of-c-sharp-psobjects-to-powershell-pipeline
        /// </summary>
        /// <param name="agents"></param>
        /// <param name="description"></param>
        /// <param name="distributionGroup"></param>
        /// <param name="identity"></param>
        /// <param name="isDistributionGroup"></param>
        /// <param name="isGroupAgentSignIn"></param>
        /// <param name="isGroupAgents"></param>
        /// <param name="name"></param>
        /// <param name="owner"></param>
        /// <param name="participationPolicy"></param>
        /// <param name="routingMethod"></param>
        /// <returns></returns>
        public PSObject PrepareSetCsRgsGroup(ObservableCollection<AgentGroupViewModel> agents, string description, string distributionGroup, string identity, 
                        bool isDistributionGroup, bool isGroupAgentSignIn, bool isGroupAgents, string name, string owner,
                        string participationPolicy, string routingMethod, int agentAlertTime)
        {
            //Create agents PS object
            Collection<PSObject> agentsObj = new Collection<PSObject>();
            foreach (var item in agents)
            {
                PSObject obj1 = new PSObject();
                obj1.Properties.Add(new PSNoteProperty("AgentSipAddress", item.SipAddress));
                agentsObj.Add(obj1);
            }
            
            //Create PS object and return it
            PSObject groupsObj = new PSObject();
            groupsObj.Properties.Add(new PSNoteProperty("Agents", agentsObj));
            groupsObj.Properties.Add(new PSNoteProperty("Description", description));
            groupsObj.Properties.Add(new PSNoteProperty("DistributionGroup", distributionGroup));
            groupsObj.Properties.Add(new PSNoteProperty("Identity", identity));
            groupsObj.Properties.Add(new PSNoteProperty("IsDistributionGroup", isDistributionGroup));
            groupsObj.Properties.Add(new PSNoteProperty("IsGroupAgentSignIn", isGroupAgentSignIn));
            groupsObj.Properties.Add(new PSNoteProperty("IsGroupAgents", isGroupAgents));
            groupsObj.Properties.Add(new PSNoteProperty("Name", name));
            groupsObj.Properties.Add(new PSNoteProperty("OwnerPool", owner));
            groupsObj.Properties.Add(new PSNoteProperty("Name", name));
            groupsObj.Properties.Add(new PSNoteProperty("ParticipationPolicy", participationPolicy));
            groupsObj.Properties.Add(new PSNoteProperty("RoutingMethod", routingMethod));
            groupsObj.Properties.Add(new PSNoteProperty("AgentAlertTime", agentAlertTime));
            return groupsObj;


            //List<string> AgentsByUri = new List<string>();
            //string AgentsCSV = "";
            //foreach (var Agent in CurrentGroup.Agents.OrderBy(a => a.Name))
            //{
            //    AgentsCSV += Agent.SipAddress.Trim() + ",";
            //}
            //AgentsCSV = AgentsCSV.TrimEnd(',');

            //string NewRoutingGroup = "";
            //if (!string.IsNullOrEmpty(CurrentGroup.RoutingMethod))
            //{
            //    var tidyUp = CurrentGroup.RoutingMethod;
            //    tidyUp = tidyUp.TrimStart('[');
            //    tidyUp = tidyUp.TrimEnd(']');
            //    NewRoutingGroup = tidyUp.Split(',')[0].Trim();
            //}

            ////TODO: validation before going to powershell

            //var LyncService = new Lync_WCF.LyncServerManager();

            //LyncService.SetCsRgsAgentGroup(CurrentGroup.Identity, CurrentGroup.Name, CurrentGroup.Description ?? "", CurrentGroup.ParticipationPolicy,
            //    CurrentGroup.Timeout.ToString(), NewRoutingGroup, CurrentGroup.DistributionGroup, CurrentGroup.Owner, AgentsCSV);

            ////TODO: need to get result back from PsFactory and display in UI
            ////Prob need to change from VOID





        }
    }
}
