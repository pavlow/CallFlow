using System;
using System.Collections.Generic;
using System.Management.Automation;
using CallFlowManager.UI.Models;
using CallFlowManager.UI.ViewModels.Groups;
using CallFlowManager.UI.ViewModels.Queues;
using CallFlowManager.UI.ViewModels.WorkFlows;
using CallFlowManager.UI.ViewModels.Users;

namespace CallFlowManager.UI.Business
{
    public interface IDataService
    {
        List<string> Pools { get; }

        IEnumerable<CsHolidayGroup> HolidayGroups { get; }

        IList<CsBusHoursGroup> BusinessHoursGroups { get; }

        IList<CsBusHours> BusinessHours { get; }

        IEnumerable<PSObject> Queues { get; }

        IEnumerable<PSObject> Groups { get; }

        IEnumerable<PSObject> Users { get; }

        IEnumerable<PSObject> WorkFlows { get; }
        
        Dictionary<string, string> UsersList { get; }
        Dictionary<string, string> RgsAgents { get; }

        List<string> SipDomains { get; }
        IList<Number> NumbersData { get; }

        IEnumerable<CsHoliday> Holidays { get; }

        IList<QueueViewModel> QueueData { get; }

        //IList<string> QueueAgentGroups { get; }

        IList<GroupViewModel> GroupsData { get; }

        IList<UserViewModel> UsersData { get; }
        UserPoliciesViewModel UserPoliciesData { get; }

        IList<WorkFlowViewModel> WorkFlowsData { get; }

        IList<UserViewModel> GetUsers(bool refresh = false, bool refreshIfNull = true);

        IList<WorkFlowViewModel> GetWorkflows(DesignerViewModel designerViewModel, bool refresh = false, bool refreshIfNull = true);
        IList<WorkFlowViewModel> GetWorkflows();
        IList<QueueViewModel> GetQueues(bool refresh = false, bool refreshIfNull = true);
        IList<Number> GetNumberInventory(bool refresh = false, bool refreshIfNull = true);
        UserPoliciesViewModel GetUserPolicies(bool refresh = false, bool refreshIfNull = true);
        Dictionary<string, string> GetRgsAgents(bool refresh = false, bool refreshIfNull = true);
        IList<GroupViewModel> GetGroups(bool refresh = false, bool refreshIfNull = true);
        void GetHolidays(bool refresh = false, bool refreshIfNull = true);
        void GetBusinessHours(bool refresh = false, bool refreshIfNull = true);
        List<string> GetSipDomains(bool refresh = false, bool refreshIfNull = true);
        List<string> GetRegistrarPools(bool refresh = false, bool refreshIfNull = true);

        event Action<EventArgs> OnQueuesUpdated;

        event Action<EventArgs> OnGroupsUpdated;

        event Action<EventArgs> OnWorkFlowsUpdated;

        event Action<EventArgs> OnBusinessHoursUpdated;

        event Action<EventArgs> OnHolidaysUpdated;

        event Action<EventArgs> OnNumbersUpdated;
        event Action<EventArgs> OnUsersUpdated;
        event Action<EventArgs> OnUserPoliciesUpdated;

        void Add(CsHolidayGroup group);

        void Remove(CsHolidayGroup group);

        void Remove(CsBusHours group);

        void LoadBusinessHours();

        void LoadQueues();

        void LoadRgsAgents();
        void LoadGroups();

        void LoadUsers();
        void LoadUserPolicies();

        void LoadWorkFlows(DesignerViewModel designerViewModel);

        void LoadHolidays();

        void LoadRegistrarPools();
        void LoadSipDomains();

        void LoadNumberInventory();
    }
}