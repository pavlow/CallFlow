using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Threading;
using CallFlowManager.UI.Models;
using CallFlowManager.UI.ViewModels.Groups;
using CallFlowManager.UI.ViewModels.Queues;
using CallFlowManager.UI.ViewModels.WorkFlows;
using CallFlowManager.UI.ViewModels.Users;

namespace CallFlowManager.UI.Business
{
    public class TestDataService //: IDataService - currently not using this
    {
        private readonly List<CsHolidayGroup> _groups = new List<CsHolidayGroup>();

        public TestDataService()
        {
            Pools = new List<string>
            {
                "Pool 1",
                "Pool 2"
            };

            HolidayGroups = new List<CsHolidayGroup>()
            {
                //new CsHolidayGroup("Holiday Group 1", "Pool 1", "Identity 1"), 
                //new CsHolidayGroup("Holiday Group 2", "Pool 2", "Identity 2")
            };

            BusinessHoursGroups = new List<CsBusHoursGroup>()
            {
                new CsBusHoursGroup("Bs Group 1", "Pool 1", "Identity 1"),
                new CsBusHoursGroup("Bs Group 2", "Pool 2", "Identity 2")
            };

            SipDomains = new List<string>()
            {
                "Domain 1",
                "Domain 2",
                "Domain 3"
            };

            var g1 = new GroupViewModel() {Name = "Group 1", Description = "Description Group # 1"};
            g1.Agents.Clear();
            g1.Agents.Add(new AgentGroupViewModel() { Name = "agent #1", SipAddress = "01", RoutingMethod = "m-01" });
            g1.Agents.Add(new AgentGroupViewModel() { Name = "agent #2", SipAddress = "02", RoutingMethod = "m-02" });
            g1.Agents.Add(new AgentGroupViewModel() { Name = "agent #3", SipAddress = "03", RoutingMethod = "m-03" });

            var g2 = new GroupViewModel() { Name = "Group 2", Description = "Description Group # 2" };
            g2.Agents.Clear();
            g2.Agents.Add(new AgentGroupViewModel() { Name = "agent #-21", SipAddress = "01", RoutingMethod = "m - # 21" });
            g2.Agents.Add(new AgentGroupViewModel() { Name = "agent #-22", SipAddress = "02", RoutingMethod = "m - # 22" });
            

            GroupsData = new List<GroupViewModel>()
                {
                    g1, g2
                };

            var q1 = new QueueViewModel() {Name = "Queue - 1", Description = "Description -   1"};
            var q2 = new QueueViewModel() {Name = "Queue # 2", Description = "Description -  2"};
            var q3 = new QueueViewModel() {Name = "Queue N 3", Description = "Description - 3"};
            q1.Groups.Add((new QueueGroupViewModel("agent 1", "01:11", "policy - 1", "routingMethod 1")));
            q1.Groups.Add((new QueueGroupViewModel("agent 2", "02:22", "policy - 2", "routingMethod 2")));
            q1.Groups.Add((new QueueGroupViewModel("agent 3", "03:33", "policy - 3", "routingMethod 3")));
            q2.Groups.Add((new QueueGroupViewModel("agent 21", "2:21", "policy - 21", "routingMethod 21")));
            q2.Groups.Add((new QueueGroupViewModel("agent 22", "2:22", "policy - 22", "routingMethod 22")));
            q3.Groups.Add((new QueueGroupViewModel("agent 31", "3:31", "policy - 31", "routingMethod 31")));

            QueueData = new List<QueueViewModel>()
                {
                    q1, q2, q3
                };


            var wf1 = new WorkFlowViewModel()
            {
                AfterHoursMessage = "AH_Message 1",
                AfterHoursSipDomain = "AH_Sip_domain1",
                EnableIVRMode = true,
                Name = "WF # 01",
                Description = "WF Description -1"
            };
            var wf2 = new WorkFlowViewModel()
            {
                AfterHoursMessage = "AH_Message 2",
                AfterHoursSipDomain = "AH_Sip_domain2",
                EnableIVRMode = true,
                Name = "WF # 02",
                Description = "WF Description -2"
            };
            var wf3 = new WorkFlowViewModel()
            {
                AfterHoursMessage = "AH_Message 3",
                AfterHoursSipDomain = "AH_Sip_domain3",
                EnableIVRMode = true,
                Name = "WF # 03",
                Description = "WF Description -3"
            };

            //var wf1 = new WorkFlowViewModel(new DesignerViewModel()) {AfterHoursMessage = "AH_Message 1", AfterHoursSipDomain = "AH_Sip_domain1", EnableIVRMode = true,
            //        Name = "WF # 01", Description = "WF Description -1"};
            //var wf2 = new WorkFlowViewModel(new DesignerViewModel())
            //{
            //    AfterHoursMessage = "AH_Message 2",
            //    AfterHoursSipDomain = "AH_Sip_domain2",
            //    EnableIVRMode = true,
            //        Name = "WF # 02", Description = "WF Description -2"};
            //var wf3 = new WorkFlowViewModel(new DesignerViewModel())
            //{
            //    AfterHoursMessage = "AH_Message 3",
            //    AfterHoursSipDomain = "AH_Sip_domain3",
            //    EnableIVRMode = true,
            //        Name = "WF # 03", Description = "WF Description -3"
            //};

            var ivr1 = new IvrViewModel(wf1) { Name = "Name 1", Parent = null, Number = 8 };
            var ivr2 = new IvrViewModel(wf1) { Name = "Name 2", Parent = null, Number = 6 };
            var ivr3 = new IvrViewModel(wf1) { Name = "Name 3", Parent = null, Number = 5 };
            var ivr4 = new IvrViewModel(wf1) { Name = "Name 4", Parent = ivr1, Number = 2 };
            var ivr5 = new IvrViewModel(wf1) { Name = "Name 5", Parent = ivr1, Number = 1 };
            var ivr6 = new IvrViewModel(wf1) { Name = "Name 6", Parent = ivr1, Number = 2 };
            var ivr7 = new IvrViewModel(wf1) { Name = "Name 7", Parent = ivr1, Number = 3 };
            ivr1.ChildIvrNodes.Add(ivr4);
            ivr1.ChildIvrNodes.Add(ivr5);
            ivr1.ChildIvrNodes.Add(ivr6);
            ivr1.ChildIvrNodes.Add(ivr7);

            var ivr31 = new IvrViewModel(wf3) { Name = "Name 31", Parent = null, Number = 1};
            var ivr34 = new IvrViewModel(wf3) { Name = "Name 34", Parent = ivr31, Number = 4 };
            var ivr36 = new IvrViewModel(wf3) { Name = "Name 36", Parent = ivr31, Number = 8 };
            var ivr37 = new IvrViewModel(wf3) { Name = "Name 37", Parent = ivr31, Number = 4 };
            ivr31.ChildIvrNodes.Add(ivr34);
            ivr31.ChildIvrNodes.Add(ivr36);
            ivr31.ChildIvrNodes.Add(ivr37);
            wf1.IvrOptions.Add(ivr1);
            wf1.IvrOptions.Add(ivr2);
            wf1.IvrOptions.Add(ivr3);
            wf3.IvrOptions.Add(ivr31);

            WorkFlowsData = new List<WorkFlowViewModel>()
                {
                    wf1, wf2, wf3
                };
        }

        public List<string> Pools { get; private set; }


        public Dictionary<string, string> UsersList { get; private set; }
        public List<string> SipDomains { get; private set; }
        public IList<Number> NumbersData { get; private set; }
        public IEnumerable<CsHoliday> Holidays { get; private set; }

        public IList<GroupViewModel> GroupsData { get; private set; }
        public IList<UserViewModel> UsersData { get; private set; }
        public UserPoliciesViewModel UserPoliciesData { get; private set; }
        public Dictionary<string, string> RgsAgents { get; private set; }

        public event Action<EventArgs> OnQueuesUpdated;
        public event Action<EventArgs> OnGroupsUpdated;
        public event Action<EventArgs> OnWorkFlowsUpdated;
        public event Action<EventArgs> OnBusinessHoursUpdated;
        public event Action<EventArgs> OnHolidaysUpdated;
        public event Action<EventArgs> OnNumbersUpdated;
        public event Action<EventArgs> OnUsersUpdated;
        public event Action<EventArgs> OnUserPoliciesUpdated;
        public IList<QueueViewModel> QueueData { get; private set; }

        //public IList<string> QueueAgentGroups { get; private set; }
        public IList<WorkFlowViewModel> WorkFlowsData { get; private set; }

        public IEnumerable<CsHolidayGroup> HolidayGroups { get; private set; }
        public IList<CsBusHoursGroup> BusinessHoursGroups { get; private set; }
        public IList<CsBusHours> BusinessHours { get; private set; }
        public IEnumerable<PSObject> Queues { get; private set; }
        public IEnumerable<PSObject> Groups { get; private set; }
        public IEnumerable<PSObject> Users { get; private set; }
        public IEnumerable<PSObject> WorkFlows { get; private set; }

        public void Add(CsHolidayGroup @group)
        {
            _groups.Add(@group);
        }

        public void Add(CsBusHours @group)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(CsHolidayGroup @group)
        {
            throw new System.NotImplementedException();
        }

        public void Remove(CsBusHours @group)
        {
            throw new System.NotImplementedException();
        }

        public void LoadBusinessHours()
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        public void LoadQueues()
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
            if (OnQueuesUpdated != null) OnQueuesUpdated.Invoke(new EventArgs());
            // QueueData = new List<QueueViewModel>();
        }

        public void LoadRgsAgents()
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }
        public void LoadGroups()
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        public void LoadUsers()
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        public void LoadUserPolicies()
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }


        public void LoadWorkFlows(DesignerViewModel designerViewModel)
        {
            if (OnWorkFlowsUpdated != null) OnWorkFlowsUpdated.Invoke(new EventArgs());
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        public void LoadHolidays()
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        public void LoadRegistrarPools()
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        public void LoadSipDomains()
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        public void LoadNumberInventory()
        {
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        
    }
}