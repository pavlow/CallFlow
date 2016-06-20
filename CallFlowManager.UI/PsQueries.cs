using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Threading;
using CallFlowManager.UI.ViewModels.BusinessHours;
using Lync_WCF;

namespace CallFlowManager.UI
{
    public class PsQueries
    {
        PsExecutor pS = new PsExecutor();
        public ICollection<PSObject> Workflows;
        public ICollection<PSObject> Queues;
        public Dictionary<string, string> CsQueuesList;
        public ICollection<PSObject> Groups;
        public ICollection<PSObject> BusHours;
        public ICollection<PSObject> RegistrarPools;
        public ICollection<PSObject> Users;
        public ICollection<PSObject> UserPolicies;
        public Collection<PSObject> UserRgsGrpMembership;
        public ICollection<PSObject> Holidays;
        public ICollection<PSObject> NumberInventory;
        private LyncServerManager _lyncServerMan = new LyncServerManager();


        public Dictionary<string, string> UsersList { get; set; }
        //public Collection<PSObject> SipDomains;
        public ICollection<PSObject> SipDomains { get; set; }
        //public List<string> SipDomainsList;
        public List<string> SipDomainsList { get; set; }

        public List<string> BusinessHoursGroups { get; set; }

        public List<string> RegistrarPoolsList { get; set; }

        //AT
        public ICollection<PSObject> UnassignedNumbers;
        //AT - Andrew Voice Features testing

        //IsLoading
        private bool _getCsWorkflowsIsLoading;
        private bool _getCsQueuesIsLoading;
        private bool _getCsGroupsIsLoading;
        private bool _getCsBusHoursIsLoading;
        private bool _getCsHolidaysIsLoading;
        private bool _getCsUsersIsLoading;
        private bool _getRegistrarPoolsIsLoading;
        private bool _getSipDomainsIsLoading;
        private bool _getNumberInventoryIsLoading;
        private bool _getCsUserPoliciesIsLoading;
        
        //NEW LOGIC

        //SETS

        public void SetCsRgsWf(object wfPsObject)
        {
            //var lyncServerMan = new LyncServerManager();
            _lyncServerMan.SetCsRgsWf(wfPsObject);
        }

        public void SetCsRgsWf(PSObject wfPsObject)
        {
            //var lyncServerMan = new LyncServerManager();
            _lyncServerMan.SetCsRgsWf(wfPsObject);
        }


        public void SetCsRgsQueue(PSObject queuePsObject)
        {
            //var lyncServerMan = new LyncServerManager();
            _lyncServerMan.SetCsRgsQueue(queuePsObject);
        }


        public void SetCsRgsAgentGroup(PSObject grpPsObject)
        {
            //var lyncServerMan = new LyncServerManager();
            _lyncServerMan.SetCsRgsAgentGroup(grpPsObject);

        }

        public void SetCsRgsHoliday(PSObject holPsObject)
        {
            //var lyncServerMan = new LyncServerManager();
            _lyncServerMan.SetCsRgsHoliday(holPsObject);

        }

        public void SetCsRgsHoursOfBusiness(PSObject busHourPsObject)
        {
           //var lyncServerMan = new LyncServerManager();
           _lyncServerMan.SetCsRgsHoursOfBusiness(busHourPsObject);

        }

        //GETS
        public ICollection<PSObject> GetNumberInventory(bool refresh = true)
        {
            if (!_getNumberInventoryIsLoading)
            {
                _getNumberInventoryIsLoading = true;
                //If NumberInventory is null or refresh requested
                if (NumberInventory == null || refresh)
                {
                    //Get NumberInventory
                    //var lyncServerMan = new LyncServerManager();
                    NumberInventory = _lyncServerMan.GetNumberInventory();
                }
                _getNumberInventoryIsLoading = false;
            }
            else if (_getNumberInventoryIsLoading)
            {
                while (_getNumberInventoryIsLoading)
                {
                    Thread.Sleep(2000);
                }
            }
            return NumberInventory;
        }


        public ICollection<PSObject> GetCsWorkflows(bool refresh = true)
        {
            if (!_getCsWorkflowsIsLoading)
            {
                _getCsWorkflowsIsLoading = true;
                //If Workflows is null or refresh requested
                if (Workflows == null || refresh)
                {
                    //Get Workflows
                    //var lyncServerMan = new LyncServerManager();
                    Workflows = _lyncServerMan.GetCsRgsWorkflow();
                }
                _getCsWorkflowsIsLoading = false;
            }
            else if (_getCsWorkflowsIsLoading)
            {
                while (_getCsWorkflowsIsLoading)
                {
                    Thread.Sleep(2000);
                }
            }
            return Workflows;
        }


        public ICollection<PSObject> GetCsHolidays(bool refresh = true)
        {
            if (!_getCsHolidaysIsLoading)
            {
                _getCsHolidaysIsLoading = true;
                //If Holidays is null or refresh requested
                if (Holidays == null || refresh)
                {
                    //Get Holidays
                    //var lyncServerMan = new LyncServerManager();
                    Holidays = _lyncServerMan.GetCsRgsHolidaySet();
                }
                _getCsHolidaysIsLoading = false;
            }
            else if (_getCsHolidaysIsLoading)
            {
                while (_getCsHolidaysIsLoading)
                {
                    Thread.Sleep(2000);
                }
            }
            return Holidays;
        }


        public ICollection<PSObject> GetSipDomains(bool refresh = true)
        {
            if (!_getSipDomainsIsLoading)
            {
                _getSipDomainsIsLoading = true;
                //If SipDomains is null or refresh requested
                if (SipDomains == null || refresh)
                {
                    //Get SipDomains
                    //var lyncServerMan = new LyncServerManager();
                    SipDomains = _lyncServerMan.GetCsSipDomain();
                }
                _getSipDomainsIsLoading = false;
            }
            else if (_getSipDomainsIsLoading)
            {
                while (_getSipDomainsIsLoading)
                {
                    Thread.Sleep(2000);
                }
            }
            return SipDomains;
        }

        public ICollection<PSObject> GetRegistrarPools(bool refresh = true)
        {
            if (!_getRegistrarPoolsIsLoading)
            {
                _getRegistrarPoolsIsLoading = true;
                //If RegistrarPools is null or refresh requested
                if (RegistrarPools == null || refresh)
                {
                    //Get RegistrarPools
                    //var lyncServerMan = new LyncServerManager();
                    RegistrarPools = _lyncServerMan.GetCsPool_Registrar();
                }
                _getRegistrarPoolsIsLoading = false;
            }
            else if (_getRegistrarPoolsIsLoading)
            {
                while (_getRegistrarPoolsIsLoading)
                {
                    Thread.Sleep(2000);
                }
            }
            return RegistrarPools;
        }
       
        //move to service or viewmodel
        //public void UpdateRegistrarPoolsList()
        //{
        //    RegistrarPoolsList = new List<string>();

        //    foreach (dynamic registrarPools in RegistrarPools)
        //    {
        //        if (registrarPools != null)
        //        {
        //            RegistrarPoolsList.Add(registrarPools.Fqdn);
        //        }
        //    }
        //}





        public ICollection<PSObject> GetCsQueues(bool refresh = true)
        {
            if (!_getCsQueuesIsLoading)
            {
                _getCsQueuesIsLoading = true;
                //If Queues is null or refresh requested
                if (Queues == null || refresh)
                {
                    //Get Groups
                    //var lyncServerMan = new LyncServerManager();
                    Queues = _lyncServerMan.GetCsRgsQueue();
                    //UpdateCsQueuesList();
                }
                _getCsQueuesIsLoading = false;
            }
            else if (_getCsQueuesIsLoading)
            {
                while (_getCsQueuesIsLoading)
                {
                    Thread.Sleep(2000);
                }
            }
            return Queues;
        }

        public ICollection<PSObject> GetCsUsers(bool refresh = true)
        {
            if (!_getCsUsersIsLoading)
            {
                _getCsUsersIsLoading = true;
                //If Users is null or refresh requested
                if (Users == null || refresh)
                {
                    //Get Users
                    //var lyncServerMan = new LyncServerManager();
                    Users = _lyncServerMan.GetCsUser();
                }
                _getCsUsersIsLoading = false;
            }
            else if (_getCsUsersIsLoading)
            {
                while (_getCsUsersIsLoading)
                {
                    Thread.Sleep(2000);
                }
            }
            return Users;
        }

        public ICollection<PSObject> GetCsUserPolicies(bool refresh = true)
        {
            if (!_getCsUserPoliciesIsLoading)
            {
                _getCsUserPoliciesIsLoading = true;
                //If UserPolicies is null or refresh requested
                if (UserPolicies == null || refresh)
                {
                    //Get User Policies
                    //var lyncServerMan = new LyncServerManager();
                    UserPolicies = _lyncServerMan.GetCsUserPolicies();
                }
                _getCsUserPoliciesIsLoading = false;
            }
            else if (_getCsUserPoliciesIsLoading)
            {
                while (_getCsUserPoliciesIsLoading)
                {
                    Thread.Sleep(2000);
                }
            }
            return UserPolicies;
        }



        public ICollection<PSObject> GetCsGroups(bool refresh = true)
        {
            if (!_getCsGroupsIsLoading)
            {
                _getCsGroupsIsLoading = true;
                //If Groups is null or refresh requested
                if (Groups == null || refresh)
                {
                    //Get Groups
                    //var lyncServerMan = new LyncServerManager();
                    Groups = _lyncServerMan.GetCsRgsAgentGroup();
                }
                _getCsGroupsIsLoading = false;
            }
            else if (_getCsGroupsIsLoading)
            {
                while (_getCsGroupsIsLoading)
                {
                    Thread.Sleep(2000);
                }
            }
            return Groups;
        }

        public ICollection<PSObject> GetCsBusHours(bool refresh=true)
        {
            if (!_getCsBusHoursIsLoading)
            {
                _getCsBusHoursIsLoading = true;
                //If BusHours is null or refresh requested
                if (BusHours == null || refresh)
                {
                    //Get BusinessHours
                    //var lyncServerMan = new LyncServerManager();
                    BusHours = _lyncServerMan.GetCsRgsHoursOfBusiness();
                }
                _getCsBusHoursIsLoading = false;
            }
            else if (_getCsBusHoursIsLoading)
            {
                while (_getCsBusHoursIsLoading)
                {
                    Thread.Sleep(2000);
                }
            }
            return BusHours;
        }

        //public void UpdateBusinessHoursGroups()
        //{
        //    BusinessHoursGroups = new List<string>();

        //    foreach (dynamic busHour in BusHours)
        //    {
        //        if (busHour != null)
        //        {
        //            BusinessHoursGroups.Add(busHour.Name);
        //        }
        //    }
        //}




        //OLD LOGIC
       
        public void GetCsUnassignedNumbers()
        {
            //var lyncServerMan = new LyncServerManager();
            UnassignedNumbers = _lyncServerMan.GetCsUnassignedNumber();
        }

        //public void GetCsWorkflows()
        //{
        //    var lyncServerMan = new LyncServerManager();
        //    Workflows = lyncServerMan.GetCsRgsWorkflow();
        //}

        //public void GetCsQueues()
        //{
        //    var lyncServerMan = new LyncServerManager();
        //    Queues = lyncServerMan.GetCsRgsQueue();            
        //    UpdateCsQueuesList();
        //}

        //public void UpdateCsQueuesList()
        //{
        //    CsQueuesList = new Dictionary<string, string>();

        //    foreach (dynamic queueName in Queues)
        //    {
        //        if (queueName != null)
        //        {
        //            CsQueuesList.Add(queueName.Identity.ServiceId.FullName.ToString() + "/" +
        //                            queueName.Identity.InstanceId.ToString(), queueName.Name.ToString());
        //        }
        //    }
        //}


        //public void GetCsGroups()
        //{
        //    var lyncServerMan = new LyncServerManager();
        //    Groups = lyncServerMan.GetCsRgsAgentGroup();
        //}

        //public void GetCsUsers()
        //{
        //    var lyncServerMan = new LyncServerManager();
        //    Users = lyncServerMan.GetCsUser();
        //}

        //public void UpdateUsersList()
        //{
        //    UsersList = new Dictionary<string, string>();

        //    foreach (dynamic user in Users)
        //    {
        //        if (user != null)
        //        {
        //            UsersList.Add(user.Name + " (" + user.SipAddress + ")", user.SipAddress);
        //        }
        //    }
        //}

        public void GetCsUserRgsGrpMembership(string sipAddress)
        {
            //UserRgsGrpMembership = RunQuery("Get-CsRgsAgentGroup | Where {$_.AgentsByUri -Contains '" + sipAddress + "'} | Select-Object Name;");
        }
        
        //public void GetRegistrarPools()
        //{
        //   var lyncServerMan = new LyncServerManager();
        //    RegistrarPools = lyncServerMan.GetCsPool_Registrar();

        //    if (RegistrarPools != null)
        //    {
        //        UpdateRegistrarPoolsList();
        //    }
        //}

        //public void UpdateRegistrarPoolsList()
        //{
        //    RegistrarPoolsList = new List<string>();

        //    foreach (dynamic registrarPools in RegistrarPools)
        //    {
        //        if (registrarPools != null)
        //        {
        //            RegistrarPoolsList.Add(registrarPools.Fqdn);
        //        }
        //    }
        //}

        //public void GetSipDomains()
        //{
        //    var lyncServerMan = new LyncServerManager();
        //    SipDomains = lyncServerMan.GetCsSipDomain();
        //}

        //public void UpdateSipDomainsList()
        //{
        //    SipDomainsList = new List<string>();

        //    foreach (dynamic sipDomain in SipDomains)
        //    {
        //        if (sipDomain != null)
        //        {
        //            SipDomainsList.Add(sipDomain.Name);
        //        }
        //    }
        //}
        
        //public void GetCsHolidays()
        //{
        //   var lyncServerMan = new LyncServerManager();
        //    Holidays = lyncServerMan.GetCsRgsHolidaySet();
        //}


        //public void GetCsBusHours()
        //{
        //    var lyncServerMan = new LyncServerManager();
        //    BusHours = lyncServerMan.GetCsRgsHoursOfBusiness();
        //}

        //public void UpdateBusinessHoursGroups()
        //{
        //    BusinessHoursGroups = new List<string>();

        //    foreach (dynamic busHour in BusHours)
        //    {
        //        if (busHour != null)
        //        {
        //            BusinessHoursGroups.Add(busHour.Name);
        //        }
        //    }
        //}
    }
}
