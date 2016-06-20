using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using CallFlowManager.UI.Models;
using CallFlowManager.UI.ViewModels.Groups;
using CallFlowManager.UI.ViewModels.Queues;
using CallFlowManager.UI.ViewModels.WorkFlows;
using CallFlowManager.UI.ViewModels.Users;
using Lync_WCF;

namespace CallFlowManager.UI.Business
{
    public class PowerShellDataService : IDataService
    {
        public PowerShellDataService()
        {
            //Instansiate PS services and pass in the data query (PsQueries)
            _businessHoursService = new BusinessHoursService();
            _holidayService = new HolidayService(DataQueries);
            _groupService = new GroupsService();
            _userService = new UsersService(DataQueries);
            _userPoliciesService = new UserPoliciesGetService();
            _queueService = new QueuesService();
            _workflowService = new WorkflowsGetService();
            _numbersService = new NumbersService();

            //Subscribe to events
            OnQueuesUpdated = delegate {  };
            OnGroupsUpdated = delegate { };
            OnUsersUpdated = delegate { };
            OnWorkFlowsUpdated = delegate { };
            OnBusinessHoursUpdated = delegate { };
            OnHolidaysUpdated = delegate { };
            OnNumbersUpdated = delegate { };
        }

        public void LoadNumberInventory()
        {
            var csNumbers = _psQueries.GetNumberInventory(true);
            _numbersService.ProcessPsNumbersInventory(csNumbers);
            NumbersData = _numbersService.LoadedNumbers ?? new List<Number>();
        }

        
        public IList<Number> GetNumberInventory(bool refresh = false, bool refreshIfNull = true)
        {
            //If null & refreshIfNull or refresh, then load
            if (NumbersData == null && refreshIfNull || refresh)
            {
                bool wasLoading = false;
                //Check if its already loading
                if (_getNumberInventoryIsLoading)
                {
                    wasLoading = true;
                    //If already loading, wait until completed
                    while (_getNumberInventoryIsLoading)
                    {
                        Thread.Sleep(2000);
                    }
                }
                //If its not loading
                if (!_getNumberInventoryIsLoading)
                {
                    //If it wasnt previously loading or its null, then load
                    if (!wasLoading || Pools == null)
                    {
                        _getNumberInventoryIsLoading = true;
                        LoadNumberInventory();
                        _getNumberInventoryIsLoading = false;
                    }
                }
            }
            return NumbersData;

            //If null & refreshIfNull or refresh requested
            //    if (NumbersData == null && refreshIfNull || refresh)
            //    {
            //        //If already loading, wait until completed
            //        while (_getNumberInventoryIsLoading)
            //        {
            //            Thread.Sleep(2000);
            //        }
            //        //If not loading, load
            //        if (!_getNumberInventoryIsLoading)
            //        {
            //            _getNumberInventoryIsLoading = true;
            //            LoadNumberInventory();
            //        }
            //        _getNumberInventoryIsLoading = false;
            //    }
            //    return NumbersData;
        }


        public void LoadUsers()
        {
            var csUsers = _psQueries.GetCsUsers(true);

            UsersData = new ObservableCollection<UserViewModel>(_userService.ProcessPsUsers(csUsers));
            if (OnUsersUpdated != null) OnUsersUpdated.Invoke(new EventArgs());
        }

        public IList<UserViewModel> GetUsers(bool refresh = false, bool refreshIfNull = true)
        {
            //If null & refreshIfNull or refresh, then load
            if (UsersData == null && refreshIfNull || refresh)
            {
                bool wasLoading = false;
                //Check if its already loading
                if (_getCsUsersIsLoading)
                {
                    wasLoading = true;
                    //If already loading, wait until completed
                    while (_getCsUsersIsLoading)
                    {
                        Thread.Sleep(2000);
                    }
                }
                //If its not loading
                if (!_getCsUsersIsLoading)
                {
                    //If it wasnt previously loading or its null, then load
                    if (!wasLoading || UsersData == null)
                    {
                        _getCsUsersIsLoading = true;
                        LoadUsers();
                        _getCsUsersIsLoading = false;
                    }
                }
            }
            return UsersData;
            ////If null & refreshIfNull or refresh requested
            //if (UsersData == null && refreshIfNull || refresh)
            //{
            //    //If already loading, wait until completed
            //    while (_getCsUsersIsLoading)
            //    {
            //        Thread.Sleep(2000);
            //    }
            //    //If not loading, load
            //    if (!_getCsUsersIsLoading)
            //    {
            //        _getCsUsersIsLoading = true;
            //        LoadUsers();
            //    }
            //    _getCsUsersIsLoading = false;
            //}
            //return UsersData;
        }


        public void LoadUserPolicies()
        {
            var csUserPolicies = _psQueries.GetCsUserPolicies(true);
            UserPoliciesData = _userPoliciesService.ProcessPsUserPolicies(csUserPolicies);
            if (OnUserPoliciesUpdated != null) OnUserPoliciesUpdated.Invoke(new EventArgs());
        }


        public UserPoliciesViewModel GetUserPolicies(bool refresh = false, bool refreshIfNull = true)
        {
            //If null & refreshIfNull or refresh, then load
            if (UserPoliciesData == null && refreshIfNull || refresh)
            {
                bool wasLoading = false;
                //Check if its already loading
                if (_getUserPoliciesIsLoading)
                {
                    wasLoading = true;
                    //If already loading, wait until completed
                    while (_getUserPoliciesIsLoading)
                    {
                        Thread.Sleep(2000);
                    }
                }
                //If its not loading
                if (!_getUserPoliciesIsLoading)
                {
                    //If it wasnt previously loading or its null, then load
                    if (!wasLoading || UserPoliciesData == null)
                    {
                        _getUserPoliciesIsLoading = true;
                        LoadUserPolicies();
                        _getUserPoliciesIsLoading = false;
                    }
                }
            }
            return UserPoliciesData;
        }


        public void LoadWorkFlows(DesignerViewModel designerViewModel)
        //public void LoadWorkFlows(DesignerViewModel designerViewModel)
        {
            //Load Dependencies (Loading first as Queues are required by WorkflowsGetService
            GetQueues();

            //OLD:
            //LoadRegistrarPools();
            //LoadSharedData();
            //LoadQueues();            

            var csWorkflows = _psQueries.GetCsWorkflows(true);
            _workflowService.ProcessPsWorkflows(csWorkflows, designerViewModel);

           //Load other dependancies
            GetRegistrarPools();
            GetBusinessHours();
            GetHolidays();
            
            WorkFlowsData = _workflowService.LoadedWorkflows ?? new List<WorkFlowViewModel>();
            //WorkFlows = _workflowService.LoadedWorkflows;

            if (OnWorkFlowsUpdated != null) OnWorkFlowsUpdated.Invoke(new EventArgs());

            
        }


        public IList<WorkFlowViewModel> GetWorkflows(DesignerViewModel designerViewModel, bool refresh = false, bool refreshIfNull = true)
        {
            //If null & refreshIfNull or refresh, then load
            if (WorkFlowsData == null && refreshIfNull || refresh)
            {
                bool wasLoading = false;
                //Check if its already loading
                if (_getCsWorkflowsIsLoading)
                {
                    wasLoading = true;
                    //If already loading, wait until completed
                    while (_getCsWorkflowsIsLoading)
                    {
                        Thread.Sleep(2000);
                    }
                }
                //If its not loading
                if (!_getCsWorkflowsIsLoading)
                {
                    //If it wasnt previously loading or its null, then load
                    if (!wasLoading || WorkFlowsData == null)
                    {
                        _getCsWorkflowsIsLoading = true;
                        LoadWorkFlows(designerViewModel);
                        _getCsWorkflowsIsLoading = false;
                    }
                }
            }
            return WorkFlowsData;
            ////If null & refreshIfNull or refresh requested
            //if (WorkFlowsData == null && refreshIfNull || refresh)
            //{
            //    //If already loading, wait until completed
            //    while (_getCsWorkflowsIsLoading)
            //    {
            //        Thread.Sleep(2000);
            //    }
            //    //If not loading, load
            //    if (!_getCsWorkflowsIsLoading)
            //    {
            //        _getCsWorkflowsIsLoading = true;
            //        //LoadWorkFlows(); - NEED TO FIX REQUIREMENT TO PASS IN WF VIEWMODEL
            //    }
            //    _getCsWorkflowsIsLoading = false;
            //}
            //return WorkFlowsData;
        }

        public IList<WorkFlowViewModel> GetWorkflows()
        {
            return WorkFlowsData;
        }


        public void LoadQueues()
        {
            var csQueues = _psQueries.GetCsQueues(true);
            var csGroups = _psQueries.GetCsGroups(false); //need to make sure users dont get queried multiple times

            //_groupService.ProcessRgsAgentGroups(csGroups);
            _queueService.ProcessPsQueues(csQueues, csGroups);

            //LoadSharedData(); //TO DO: Move to own process independant of tab load
            ////Load dependancies
            //LoadSipDomains();
            //LoadRegistrarPools();

            //Load dependancies
            GetSipDomains();
            GetRegistrarPools();

            //QueueAgentGroups = _groupService.LoadedAgentGroups;
            QueueData = _queueService.LoadedQueues ?? new List<QueueViewModel>();
            if (OnQueuesUpdated != null) OnQueuesUpdated.Invoke(new EventArgs());

        }

        public IList<QueueViewModel> GetQueues(bool refresh = false, bool refreshIfNull = true)
        {
            //If null & refreshIfNull or refresh, then load
            if (QueueData == null && refreshIfNull || refresh)
            {
                bool wasLoading = false;
                //Check if its already loading
                if (_getCsQueuesIsLoading)
                {
                    wasLoading = true;
                    //If already loading, wait until completed
                    while (_getCsQueuesIsLoading)
                    {
                        Thread.Sleep(2000);
                    }
                }
                //If its not loading
                if (!_getCsQueuesIsLoading)
                {
                    //If it wasnt previously loading or its null, then load
                    if (!wasLoading || QueueData == null)
                    {
                        _getCsQueuesIsLoading = true;
                        LoadQueues();
                        _getCsQueuesIsLoading = false;
                    }
                }
            }
            return QueueData;
            ////If null & refreshIfNull or refresh requested
            //if (QueueData == null && refreshIfNull|| refresh)
            //{
            //    //If already loading, wait until completed
            //    while (_getCsQueuesIsLoading)
            //    {
            //        Thread.Sleep(2000);
            //    }
            //    //If not loading, load
            //    if (!_getCsQueuesIsLoading)
            //    {
            //        _getCsQueuesIsLoading = true;
            //        LoadQueues();
            //    }
            //    _getCsQueuesIsLoading = false;
            //}
            //return QueueData;
        }


        public void LoadRgsAgents()
        {
            var csRgsAgents = _psQueries.GetCsUsers(false); //need to make sure users dont get queried multiple times
            _userService.CreateRgsAgentList(csRgsAgents);

            RgsAgents = _userService.RgsAgentList ?? new Dictionary<string, string>();
            //Not sure how this works yet, or if its required - working as is without:
            //if (OnGroupsUpdated != null) OnGroupsUpdated.Invoke(new EventArgs());
        }

        public Dictionary<string, string>GetRgsAgents(bool refresh = false, bool refreshIfNull = true)
        {
            //If null & refreshIfNull or refresh, then load
            if (RgsAgents == null && refreshIfNull || refresh)
            {
                bool wasLoading = false;
                //Check if its already loading
                if (_getCsRgsAgentsIsLoading)
                {
                    wasLoading = true;
                    //If already loading, wait until completed
                    while (_getCsRgsAgentsIsLoading)
                    {
                        Thread.Sleep(2000);
                    }
                }
                //If its not loading
                if (!_getCsRgsAgentsIsLoading)
                {
                    //If it wasnt previously loading or its null, then load
                    if (!wasLoading || RgsAgents == null)
                    {
                        _getCsRgsAgentsIsLoading = true;
                        LoadRgsAgents();
                        _getCsRgsAgentsIsLoading = false;
                    }
                }
            }
            return RgsAgents;
        }




        public void LoadGroups()
        {
            var csGroups = _psQueries.GetCsGroups(true);
            var csUsers = _psQueries.GetCsUsers(false); //make sure users dont get queried multiple times

            _groupService.ProcessPsGroups(csGroups, csUsers);

           ////Dependencies
           // LoadRgsAgents();
           // LoadRegistrarPools();

            //Dependencies
            GetRgsAgents();
            GetRegistrarPools();

            GroupsData = _groupService.LoadedGroups ?? new ObservableCollection<GroupViewModel>().ToList();
            if (OnGroupsUpdated != null) OnGroupsUpdated.Invoke(new EventArgs());

            

        }

       public IList<GroupViewModel> GetGroups(bool refresh = false, bool refreshIfNull = true)
       {
           //If null & refreshIfNull or refresh, then load
           if (GroupsData == null && refreshIfNull || refresh)
           {
               bool wasLoading = false;
               //Check if its already loading
               if (_getCsGroupsIsLoading)
               {
                   wasLoading = true;
                   //If already loading, wait until completed
                   while (_getCsGroupsIsLoading)
                   {
                       Thread.Sleep(2000);
                   }
               }
               //If its not loading
               if (!_getCsGroupsIsLoading)
               {
                   //If it wasnt previously loading or its null, then load
                   if (!wasLoading || GroupsData == null)
                   {
                       _getCsGroupsIsLoading = true;
                       LoadGroups();
                       _getCsGroupsIsLoading = false;
                   }
               }
           }
           return GroupsData;
           ////If null & refreshIfNull or refresh requested
           //if (GroupsData == null && refreshIfNull || refresh)
           //{
           //    //If already loading, wait until completed
           //    while (_getCsGroupsIsLoading)
           //    {
           //        Thread.Sleep(2000);
           //    }
           //    //If not loading, load
           //    if (!_getCsGroupsIsLoading)
           //    {
           //        _getCsGroupsIsLoading = true;
           //        LoadGroups();
           //    }
           //    _getCsGroupsIsLoading = false;
           //}
           //return GroupsData;
       }


       public void LoadHolidays()
       {
           var csHolidays = _psQueries.GetCsHolidays(true);

           _holidayService.ProcessPsHolidays(csHolidays);

           ////Load Dependencies
           //LoadRegistrarPools();

           //Load Dependencies
           GetRegistrarPools();

           Holidays = _holidayService.Holidays ?? new List<CsHoliday>();
           HolidayGroups = _holidayService.HolidayGroups ?? new List<CsHolidayGroup>();
           if (OnHolidaysUpdated != null) OnHolidaysUpdated.Invoke(new EventArgs());

           
       }
        
       public void GetHolidays(bool refresh = false, bool refreshIfNull = true)
       {
           //If null & refreshIfNull or refresh, then load
           if ((Holidays == null || HolidayGroups == null) && refreshIfNull || refresh)
           {
               bool wasLoading = false;
               //Check if its already loading
               if (_getCsHolidaysIsLoading)
               {
                   wasLoading = true;
                   //If already loading, wait until completed
                   while (_getCsHolidaysIsLoading)
                   {
                       Thread.Sleep(2000);
                   }
               }
               //If its not loading
               if (!_getCsHolidaysIsLoading)
               {
                   //If it wasnt previously loading or its null, then load
                   if (!wasLoading || (Holidays == null || HolidayGroups == null))
                   {
                       _getCsHolidaysIsLoading = true;
                       LoadHolidays();
                       _getCsHolidaysIsLoading = false;
                   }
               }
           }
           //return UsersData;
           ////If null & refreshIfNull or refresh requested
           //if ((Holidays == null || HolidayGroups == null) && refreshIfNull || refresh)
           //{
           //    //If already loading, wait until completed
           //    while (_getCsHolidaysIsLoading)
           //    {
           //        Thread.Sleep(2000);
           //    }
           //    //If not loading, load
           //    if (!_getCsHolidaysIsLoading)
           //    {
           //        _getCsHolidaysIsLoading = true;
           //        LoadHolidays();
           //    }
           //    _getCsHolidaysIsLoading = false;
           //}
       }

        public void LoadBusinessHours()
        {
            //Query Ps for Business Hours
            var csBusHours = _psQueries.GetCsBusHours(true);
            _businessHoursService.ProcessPsBusinessHours(csBusHours);

            ////Load Dependencies
            //LoadRegistrarPools();

            //Load Dependencies
            LoadRegistrarPools();

            BusinessHoursGroups = _businessHoursService.BusinessHoursGroups ?? new List<CsBusHoursGroup>();
            BusinessHours = _businessHoursService.BusinessHours ?? new List<CsBusHours>();
            if (OnBusinessHoursUpdated != null) OnBusinessHoursUpdated.Invoke(new EventArgs());

            
        }
       
        public void GetBusinessHours(bool refresh = false, bool refreshIfNull = true)
        {
            //If null & refreshIfNull or refresh, then load
            if ((BusinessHours == null || BusinessHoursGroups == null) && refreshIfNull || refresh)
            {
                bool wasLoading = false;
                //Check if its already loading
                if (_getCsBusinessHoursIsLoading)
                {
                    wasLoading = true;
                    //If already loading, wait until completed
                    while (_getCsBusinessHoursIsLoading)
                    {
                        Thread.Sleep(2000);
                    }
                }
                //If its not loading
                if (!_getCsBusinessHoursIsLoading)
                {
                    //If it wasnt previously loading or its null, then load
                    if (!wasLoading || (BusinessHours == null || BusinessHoursGroups == null))
                    {
                        _getCsBusinessHoursIsLoading = true;
                        LoadBusinessHours();
                        _getCsBusinessHoursIsLoading = false;
                    }
                }
            }
            //return UsersData;
            ////If null & refreshIfNull or refresh requested
            //if ((BusinessHours == null || BusinessHoursGroups == null) && refreshIfNull || refresh)
            //{
            //    //If already loading, wait until completed
            //    while (_getCsBusinessHoursIsLoading)
            //    {
            //        Thread.Sleep(2000);
            //    }
            //    //If not loading, load
            //    if (!_getCsBusinessHoursIsLoading)
            //    {
            //        _getCsBusinessHoursIsLoading = true;
            //        LoadBusinessHours();
            //    }
            //    _getCsBusinessHoursIsLoading = false;
            //}
        }
        


        public void LoadSipDomains()
        {
            //Maybe move this stuff to a service later
            var getSipDomains = _psQueries.GetSipDomains();

            if (getSipDomains != null)
            {
                SipDomains = new List<string>();
                foreach (dynamic sipDomain in getSipDomains)
                {
                    if (sipDomain != null)
                    {
                        SipDomains.Add(sipDomain.Name);
                    }
                }
            }
        }

        public List<string> GetSipDomains(bool refresh = false, bool refreshIfNull = true)
        {
            //If null & refreshIfNull or refresh, then load
            if (SipDomains == null && refreshIfNull || refresh)
            {
                bool wasLoading = false;
                //Check if its already loading
                if (_getCsSipDomainsIsLoading)
                {
                    wasLoading = true;
                    //If already loading, wait until completed
                    while (_getCsSipDomainsIsLoading)
                    {
                        Thread.Sleep(2000);
                    }
                }
                //If its not loading
                if (!_getCsSipDomainsIsLoading)
                {
                    //If it wasnt previously loading or its null, then load
                    if (!wasLoading || SipDomains == null)
                    {
                        _getCsSipDomainsIsLoading = true;
                        LoadSipDomains();
                        _getCsSipDomainsIsLoading = false;
                    }
                }
            }
            return SipDomains;
            ////If null & refreshIfNull or refresh requested
            //if (SipDomains == null && refreshIfNull || refresh)
            //{
            //    //If already loading, wait until completed
            //    while (_getCsSipDomainsIsLoading)
            //    {
            //        Thread.Sleep(2000);
            //    }
            //    //If not loading, load
            //    if (!_getCsSipDomainsIsLoading)
            //    {
            //        _getCsSipDomainsIsLoading = true;
            //        LoadSipDomains();
            //    }
            //    _getCsSipDomainsIsLoading = false;
            //}
            //return SipDomains;
        }
        

        public void LoadRegistrarPools()
        {
            //Maybe move this stuff to a service later
            var getRegistrarPools = _psQueries.GetRegistrarPools();

            if (getRegistrarPools != null)
            {
                Pools = new List<string>();
                foreach (dynamic registrarPools in getRegistrarPools)
                {
                    if (registrarPools != null)
                    {
                        Pools.Add(registrarPools.Fqdn);
                    }
                }
            }
        }
        

        public List<string> GetRegistrarPools(bool refresh = false, bool refreshIfNull = true)
        {
            //If null & refreshIfNull or refresh, then load
            if (Pools == null && refreshIfNull || refresh)
            {
                bool wasLoading = false;
                //Check if its already loading
                if (_getRegistrarPoolsIsLoading)
                {
                    wasLoading = true;
                    //If already loading, wait until completed
                    while (_getRegistrarPoolsIsLoading)
                    {
                        Thread.Sleep(2000);
                    }
                }
                //If its not loading
                if (!_getRegistrarPoolsIsLoading)
                {
                    //If it wasnt previously loading or its null, then load
                    if (!wasLoading || Pools == null)
                    {
                        _getRegistrarPoolsIsLoading = true;
                        LoadRegistrarPools();
                        _getRegistrarPoolsIsLoading = false;
                    }
                }
            }
            return Pools;
        }




        private void LoadSharedData()
        {
            //var lyncServerMan = new LyncServerManager();
            
            ////GET: Sip Domains
            ////DataQueries.GetSipDomains();
            ////DataQueries.UpdateSipDomainsList();
            ////SipDomains = DataQueries.SipDomainsList ?? new List<string>();
            //var getSipDomains = lyncServerMan.GetCsSipDomain();

            //SipDomains = new List<string>();
            //foreach (dynamic sipDomain in getSipDomains)
            //{
            //    if (sipDomain != null)
            //    {
            //        SipDomains.Add(sipDomain.Name);
            //    }
            //}
            
            ////GET: Registrar Pools
            ////DataQueries.GetRegistrarPools();
            ////Pools = DataQueries.RegistrarPoolsList ?? new List<string>();
            //var getRegistrarPools = lyncServerMan.GetCsPool_Registrar();

            //Pools = new List<string>();
            //foreach (dynamic registrarPools in getRegistrarPools)
            //{
            //    if (registrarPools != null)
            //    {
            //        Pools.Add(registrarPools.Fqdn);
            //    }
            //}
        }

        public void Add(CsHolidayGroup @group)
        {
            throw new NotImplementedException();
        }

        public void Add(CsBusHours @group)
        {
            throw new NotImplementedException();
        }

        public void Remove(CsHolidayGroup @group)
        {
            throw new NotImplementedException();
        }

        public void Remove(CsBusHours @group)
        {
            throw new NotImplementedException();
        }


        //All PowerShell data is stored and refferenced from here
        public List<string> Pools { get; private set; }
        public IEnumerable<PSObject> Queues { get; private set; }
        public IList<QueueViewModel> QueueData { get; private set; }
        //public IList<string> QueueAgentGroups { get; private set; }
        public IEnumerable<PSObject> Groups { get; private set; }
        public IEnumerable<PSObject> Users { get; private set; }
        public IEnumerable<PSObject> WorkFlows { get; private set; }
        public IList<WorkFlowViewModel> WorkFlowsData { get; private set; }
        public Dictionary<string, string> UsersList { get; private set; }
        public Dictionary<string, string> RgsAgents { get; private set; }
        public IEnumerable<CsHolidayGroup> HolidayGroups { get; private set; }
        public IEnumerable<CsHoliday> Holidays { get; private set; }
        public IList<GroupViewModel> GroupsData { get; private set; }
        public IList<UserViewModel> UsersData { get; private set; }
        public UserPoliciesViewModel UserPoliciesData { get; private set; }
        public IList<CsBusHoursGroup> BusinessHoursGroups { get; private set; }
        public IList<CsBusHours> BusinessHours { get; private set; } //public IEnumerable<string> SipDomains { get; private set; }
        public List<string> SipDomains { get; private set; }
        public IList<Number> NumbersData { get; private set; }

        //Event handlers
        public event Action<EventArgs> OnQueuesUpdated;
        public event Action<EventArgs> OnGroupsUpdated;
        public event Action<EventArgs> OnUsersUpdated;
        public event Action<EventArgs> OnUserPoliciesUpdated;
        public event Action<EventArgs> OnWorkFlowsUpdated;
        public event Action<EventArgs> OnBusinessHoursUpdated;
        public event Action<EventArgs> OnHolidaysUpdated;
        public event Action<EventArgs> OnNumbersUpdated;

        //Instansiates instance of the SfB PowerShell queries - this will be passed to all the relevant PowerShell services so there is only ever one instance/source of knowledge
        private readonly PsQueries DataQueries = new PsQueries();
        private readonly PsQueries _psQueries = new PsQueries();

        //PowerShell services responsiblie for taking the PowerShell data query (PsQueries) and processing 
        private readonly BusinessHoursService _businessHoursService;
        private readonly GroupsService _groupService;
        private readonly QueuesService _queueService;
        private readonly UsersService _userService;
        private readonly UserPoliciesGetService _userPoliciesService;
        private readonly HolidayService _holidayService;
        private readonly WorkflowsGetService _workflowService;
        private readonly NumbersService _numbersService;

        //IsLoading
        private bool _getCsWorkflowsIsLoading;
        private bool _getCsQueuesIsLoading;
        private bool _getCsGroupsIsLoading;
        private bool _getCsHolidaysIsLoading;
        private bool _getCsUsersIsLoading;
        private bool _getRegistrarPoolsIsLoading;
        private bool _getNumberInventoryIsLoading;
        private bool _getCsBusinessHoursIsLoading;
        private bool _getCsSipDomainsIsLoading;
        private bool _getUserPoliciesIsLoading;
        private bool _getCsRgsAgentsIsLoading;


    }
}
