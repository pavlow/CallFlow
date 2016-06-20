using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using CallFlowManager.UI;
using CallFlowManager.UI.ViewModels.Groups;

namespace CallFlowManager.UI.Business
{
    public class GroupsService
    {
        //Local copy of passed in Ps data object
        private ICollection<PSObject> _csGroups { get; set; }
        private ICollection<PSObject> _csUsers { get; set; }

        public List<GroupViewModel> LoadedGroups;
        public List<string> LoadedAgentGroups;

        public GroupsService()
        {
            //should move this inside Process method so its cleared each time
            LoadedGroups = new List<GroupViewModel>();
        }
        public List<string> ProcessRgsAgentGroups(ICollection<PSObject> csGroups)
        {
            _csGroups = csGroups;
            LoadedAgentGroups = new List<string>();

            if (_csGroups != null)
            {
                foreach (dynamic grp in _csGroups)
                {
                    LoadedAgentGroups.Add(grp.Name.ToString());
                }
            }
            return LoadedAgentGroups;
        }

        public IEnumerable<GroupViewModel> ProcessPsGroups(ICollection<PSObject> csGroups, ICollection<PSObject> csUsers)
       {
           _csGroups = csGroups;
           _csUsers = csUsers;
           LoadedGroups.Clear();

           foreach (dynamic grp in _csGroups)
            {
                if (grp != null)
                {
                    var newGroup = new GroupViewModel();

                    //Name
                    if (grp.Name != null)
                        newGroup.Name = grp.Name;

                    //OwnerPool
                    if (grp.OwnerPool != null)
                        newGroup.OwnerPool = grp.OwnerPool.ToString();
                    
                    //Description
                    if (grp.Description != null)
                        newGroup.Description = grp.Description;
                    //Enable Distribution Group
                    if (grp.DistributionGroupAddress != null)
                    {
                        newGroup.IsDistributionGroup = true;
                        newGroup.DistributionGroup = grp.DistributionGroupAddress;
                    }
                    //Enable Agent Group
                    else if (grp.DistributionGroupAddress == null)
                    {
                        newGroup.IsGroupAgents = true;
                    }

                    //Routing Methods
                    if (grp.RoutingMethod != null)
                        newGroup.RoutingMethod = grp.RoutingMethod.ToString();

                    //Routing Time
                    if (grp.AgentAlertTime != null)
                        newGroup.Timeout = grp.AgentAlertTime;

                    //Agent Sign-In
                    if (grp.ParticipationPolicy != null)
                    {
                        newGroup.ParticipationPolicy = grp.ParticipationPolicy.ToString();
                        if (newGroup.ParticipationPolicy == "Formal")
                        {
                            newGroup.IsGroupAgentSignIn = true;
                        }
                        else if (newGroup.ParticipationPolicy == "Informal")
                        {
                            newGroup.IsGroupAgentSignIn = false;
                        }
                    }

                    //Group Agents
                    if (grp.AgentsByUri.Count > 0)
                    {
                        foreach (var agentUri in grp.AgentsByUri)
                        {
                            if (agentUri != null)
                            {
                                var userFilter =
                                    _csUsers.Where(
                                        x => x.Members["SipAddress"].Value.ToString() == agentUri.ToString())
                                        .ToList();

                                //USER GROUP MEMBERSHIP _ COME BACK TO LATER
                                //CsQ.GetCsUserRgsGrpMembership(agentUri.ToString());
                                //if (CsQ.UserRgsGrpMembership != null)
                                //{
                                //    var test = string.Join(",", CsQ.UserRgsGrpMembership);
                                //    MessageBox.Show(string.Join(",", CsQ.UserRgsGrpMembership.ToString()));
                                //}

                                //var uri =
                                //from g in CsQ.Groups
                                //from dynamic member in group.Members
                                //where
                                //    (from agent in (IEnumerable<dynamic>)member.AgentsByUri
                                //     where agent.Name = "AgentsByUri"  // this line may be redundant
                                //     from x in (IEnumerable<dynamic>)agent.Value
                                //     select x.AbsoluteUri).Contains("SomeValue")
                                //select member.Value;

                                if (userFilter != null)
                                {
                                    foreach (dynamic user in userFilter)
                                    {
                                        newGroup.Agents.Add(new AgentGroupViewModel() { Name = user.name, SipAddress = user.sipAddress });
                                    }
                                }
                            }
                        }
                    }

                    //Owning Pool
                    if (grp.OwnerPool != null)
                        newGroup.Owner = "Owner: " + grp.OwnerPool;

                    //Identity
                    if (grp.Identity != null)
                        newGroup.Identity = "Identity: " + grp.Identity;

                    LoadedGroups.Add(newGroup);
                }
            }

            return LoadedGroups;
        }
    }
}
