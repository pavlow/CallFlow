using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace Lync_WCF
{
    public class LyncServerManager
    {
        private readonly PsFactory _factory = new LyncPsFactory();

        #region CALL FLOW MANAGER GETS

        public ICollection<PSObject> GetNumberInventory()
        {
            return _factory.RunPowerShellScriptFile("GetNumberInventory.ps1");
        }

        public ICollection<PSObject> GetCsRgsWorkflow()
        {
            return _factory.RunPowerShellScriptFile("GetCsRgsWorkflow.ps1");
        }

        public ICollection<PSObject> GetCsRgsQueue()
        {
            return _factory.RunPowerShellScriptFile("GetCsRgsQueue.ps1");
        }

        public ICollection<PSObject> GetCsRgsAgentGroup()
        {
            return _factory.RunPowerShellScriptFile("GetCsRgsAgentGroup.ps1");
        }
        public ICollection<PSObject> GetCsRgsHoursOfBusiness()
        {
            return _factory.RunPowerShellScriptFile("GetCsRgsHoursOfBusiness.ps1");
        }
                
        public ICollection<PSObject> GetCsUser()
        {
            return _factory.RunPowerShellScriptFile("GetCsUser.ps1");
        }

        public ICollection<PSObject> GetCsUserPolicies()
        {
            return _factory.RunPowerShellScriptFile("GetCsUserPolicies.ps1");
        }

        public ICollection<PSObject> GetCsRgsHolidaySet()
        {
            return _factory.RunPowerShellScriptFile("GetCsRgsHolidaySet.ps1");
        }


        public ICollection<PSObject> GetCsPool_Registrar()
        {
            return _factory.RunPowerShellScriptFile("GetCsPool_Registrar.ps1");
        }

        public ICollection<PSObject> GetCsSipDomain()
        {
            return _factory.RunPowerShellScriptFile("GetCsSipDomain.ps1");
        }

        public ICollection<PSObject> GetCsUnassignedNumber()
        {
            return _factory.RunPowerShellScriptFile("GetCsUnassignedNumber.ps1");
        }




        #endregion CALL FLOW MANAGER GETS


        #region CALL FLOW MANAGER SETS

        public ICollection<PSObject> SetCsRgsWf(object wfObj)
        {
            var parameters = new Dictionary<string, object>();
            parameters["wfObj"] = wfObj;

            return _factory.RunPowerShellScriptFile("SetCsRgsWorkflow_v2.ps1", parameters);
        }

        public ICollection<PSObject> SetCsRgsWf(PSObject wfObj)
        {
            var parameters = new Dictionary<string, object>();
            parameters["wfObj"] = wfObj;

            return _factory.RunPowerShellScriptFile("SetCsRgsWorkflow.ps1", parameters);
        }


        public ICollection<PSObject> SetCsRgsQueue(PSObject queuesObj)
        {
            var parameters = new Dictionary<string, object>();
            parameters["queuesObj"] = queuesObj;

            return _factory.RunPowerShellScriptFile("SetCsRgsQueue.ps1", parameters);
        }

        public ICollection<PSObject> SetCsRgsHoliday(PSObject holObj)
        {
            var parameters = new Dictionary<string, object>();
            parameters["HolObj"] = holObj;

            return _factory.RunPowerShellScriptFile("SetCsRgsHolidaySet.ps1", parameters);
        }

        public ICollection<PSObject> SetCsRgsHoursOfBusiness(PSObject busHoursObj)
        {
            var parameters = new Dictionary<string, object>();
            parameters["BusHoursObj"] = busHoursObj;

            return _factory.RunPowerShellScriptFile("SetCsRgsHoursOfBusiness.ps1", parameters);
        }

        public PSObject NewCsRgsCallAction(string action, string queueId=null, string uri=null, object prompt=null, object question=null)
        {
            var parameters = new Dictionary<string, object>();
            parameters["action"] = action;
            parameters["queueId"] = queueId;
            parameters["uri"] = uri;
            parameters["prompt"] = prompt;
            parameters["question"] = question;

            var result =  _factory.RunPowerShellScriptFile("NewCsRgsCallAction.ps1", parameters);
            return result.FirstOrDefault();

        }

        public ICollection<PSObject> SetCsRgsAgentGroup(PSObject grpObj)
        {
            var parameters = new Dictionary<string, object>();
            parameters["groupsObj"] = grpObj;

            return _factory.RunPowerShellScriptFile("SetCsRgsAgentGroup.ps1", parameters);
        }

        //OLD - To be removed once dependancys are removed
        public void SetCsRgsAgentGroup(string Identity, string Name, string Description, string ParticipationPolicy, string AgentAlertTime,
                                       string RoutingMethod, string DistributionGroupAddress, string OwnerPool, string AgentsByUri)
        {
            var parameters = new Dictionary<string, object>();
            parameters["Identity"] = Identity;
            parameters["Name"] = Name;
            parameters["Description"] = Description;
            parameters["ParticipationPolicy"] = ParticipationPolicy;
            parameters["AgentAlertTime"] = AgentAlertTime;
            parameters["RoutingMethod"] = RoutingMethod;
            parameters["DistributionGroupAddress"] = DistributionGroupAddress;
            parameters["OwnerPool"] = OwnerPool;
            parameters["AgentsByUri"] = AgentsByUri;

            _factory.RunPowerShellScriptFile("SetCsRgsAgentGroup.ps1", parameters);
        }

        #endregion CALL FLOW MANAGER SETS


        #region EXISTING


        public void EnableComputer()
        {
            _factory.RunPowerShellScriptFile("EnableCsComputer.ps1");
        }

        public List<string> GetRegistrarPools()
        {
            var results = _factory.RunPowerShellScriptFile("GetPools.ps1");
            return results.Select(item => item.Properties["Identity"].Value.ToString()).ToList();
        }

        public void AddSimpleUrl(string component, string domain, string url)
        {
            var parameters = new Dictionary<string, object>();
            parameters["Component"] = component;
            parameters["Domain"] = domain;
            parameters["Url"] = url;
            _factory.RunPowerShellScriptFile("AddSimpleUrl.ps1", parameters);
        }

        public void RemoveSimpleUrl(string component, string domain)
        {
            var parameters = new Dictionary<string, object>();
            parameters["Component"] = component;
            parameters["Domain"] = domain;
            _factory.RunPowerShellScriptFile("RemoveSimpleUrl.ps1", parameters);
        }

        public void AddCallLocation(string PstnUsageName, string VoiceRouteName, string DialPlanName, string DialPlanIdentity,
                                    string DialPlanDescription, string DialinConferencingRegion, string VoicePolicyName,
                                    string VoicePolicyDescription, string DialPlanArea, string PstnGateways)
        {
            var parameters = new Dictionary<string, object>();
            parameters["PstnUsageName"] = PstnUsageName;
            parameters["VoiceRouteName"] = VoiceRouteName;
            parameters["DialPlanName"] = DialPlanName;
            parameters["DialPlanIdentity"] = DialPlanIdentity;
            parameters["DialPlanDescription"] = DialPlanDescription;
            parameters["DialinConferencingRegion"] = DialinConferencingRegion;
            parameters["VoicePolicyName"] = VoicePolicyName;
            parameters["VoicePolicyDescription"] = VoicePolicyDescription;
            parameters["DialPlanArea"] = DialPlanArea;
            parameters["PstnGateways"] = PstnGateways;
            _factory.RunPowerShellScriptFile("AddCallLocation.ps1", parameters);
        }

        public void RemoveCallLocation(string PstnUsageName, string VoiceRouteName, string DialPlanIdentity, string VoicePolicyName)
        {
            var parameters = new Dictionary<string, object>();
            parameters["PstnUsageName"] = PstnUsageName;
            parameters["VoiceRouteName"] = VoiceRouteName;
            parameters["DialPlanIdentity"] = DialPlanIdentity;
            parameters["VoicePolicyName"] = VoicePolicyName;
            _factory.RunPowerShellScriptFile("RemoveCallLocation.ps1", parameters);
        }

        public void AddSubscriberContact(string name, string sipAddress, string registrarPool, string tenantOu, string accessNumber, string description, string ipPhone, string domainController)
        {
            var parameters = new Dictionary<string, object>();
            parameters["name"] = name;
            parameters["sipAddress"] = sipAddress;
            parameters["registrarPool"] = registrarPool;
            parameters["tenantOu"] = tenantOu;
            parameters["accessNumber"] = accessNumber;
            parameters["description"] = description;
            parameters["ipPhone"] = ipPhone;
            parameters["domainController"] = domainController;
            _factory.RunPowerShellScriptFile("AddSubscriberContact.ps1", parameters);
        }

        public void RemoveSubscriberContact(string sipAddress, string domainController)
        {
            var parameters = new Dictionary<string, object>();
            parameters["sipAddress"] = sipAddress;
            parameters["domainController"] = domainController;
            _factory.RunPowerShellScriptFile("RemoveSubscriberContact.ps1", parameters);
        }

        #endregion EXISTING
    }
}