using System.Collections.Generic;
using System.Management.Automation;
using CallFlowManager.UI.ViewModels.Users;

namespace CallFlowManager.UI.Business
{
    public class UsersService
    {
        //private readonly PsQueries _dataRetriever;
        private ICollection<PSObject> _csUsers { get; set; }

        public Dictionary<string, string> RgsAgentList;

        public List<UserViewModel> ProcessedUsers;

        public UsersService(PsQueries dataRetriever)
        {
            //_dataRetriever = dataRetriever;
            _csUsers = new List<PSObject>();            
        }

        //Not required:
        public IEnumerable<UserViewModel> ProcessPsUsers(ICollection<PSObject> csUsers)
        {
            _csUsers = csUsers;
            //_dataRetriever.GetCsUsers();
            //_dataRetriever.UpdateUsersList();

            ProcessedUsers = new List<UserViewModel>();
            //var usrFilter = _dataService.Users.ToList();
            
            foreach (dynamic usr in csUsers)
            {
                if (usr != null)
                {
                    var newUser = new UserViewModel();

                    //DisplayName
                    if (usr.DisplayName != null)
                        newUser.DisplayName = usr.DisplayName;

                    //SipAddress
                    if (usr.SipAddress != null)
                        newUser.SipAddress = usr.SipAddress;

                    //LineUri
                    if (usr.LineUri != null)
                        newUser.LineUri = usr.LineUri;

                    //PrivateLine
                    if (usr.PrivateLine != null)
                        newUser.PrivateLine = usr.PrivateLine;

                    //ArchivingPolicy
                    if (usr.ArchivingPolicy != null)
                        newUser.ArchivingPolicy = usr.ArchivingPolicy.ToString();

                    //CallViaWorkPolicy
                    if (usr.CallViaWorkPolicy != null)
                        newUser.CallViaWorkPolicy = usr.CallViaWorkPolicy.ToString();

                    //ClientPolicy
                    if (usr.ClientPolicy != null)
                        newUser.ClientPolicy = usr.ClientPolicy.ToString();

                    //ClientVersionPolicy
                    if (usr.ClientVersionPolicy != null)
                        newUser.ClientVersionPolicy = usr.ClientVersionPolicy.ToString();

                    //ConferencingPolicy
                    if (usr.ConferencingPolicy != null)
                        newUser.ConferencingPolicy = usr.ConferencingPolicy.ToString();

                    //DialPlan
                    if (usr.DialPlan != null)
                        newUser.DialPlan = usr.DialPlan.ToString();

                    //Enabled
                    if (usr.Enabled != null)
                        newUser.Enabled = usr.Enabled;

                    //EnterpriseVoiceEnabled
                    if (usr.EnterpriseVoiceEnabled != null)
                        newUser.EnterpriseVoiceEnabled = usr.EnterpriseVoiceEnabled;

                    //ExchangeArchivingPolicy
                    if (usr.ExchangeArchivingPolicy != null)
                        newUser.ExchangeArchivingPolicy = usr.ExchangeArchivingPolicy.ToString();

                    //ExternalAccessPolicy
                    if (usr.ExternalAccessPolicy != null)
                        newUser.ExternalAccessPolicy = usr.ExternalAccessPolicy.ToString();

                    //ExUmEnabled
                    if (usr.ExUmEnabled != null)
                        newUser.ExUmEnabled = usr.ExUmEnabled;

                    //HomeServer
                    if (usr.HomeServer != null)
                        newUser.HomeServer = usr.HomeServer.ToString();

                    //HostedVoiceMail
                    if (usr.HostedVoiceMail != null)
                        newUser.HostedVoiceMail = usr.HostedVoiceMail.ToString();

                    //HostedVoicemailPolicy
                    if (usr.HostedVoicemailPolicy != null)
                        newUser.HostedVoicemailPolicy = usr.HostedVoicemailPolicy.ToString();

                    //HostingProvider
                    if (usr.HostingProvider != null)
                        newUser.HostingProvider = usr.HostingProvider.ToString();

                    //LocationPolicy
                    if (usr.LocationPolicy != null)
                        newUser.LocationPolicy = usr.LocationPolicy.ToString();

                    //MobilityPolicy
                    if (usr.MobilityPolicy != null)
                        newUser.MobilityPolicy = usr.MobilityPolicy.ToString();

                    //PersistentChatPolicy
                    if (usr.PersistentChatPolicy != null)
                        newUser.PersistentChatPolicy = usr.PersistentChatPolicy.ToString();

                    //PinPolicy
                    if (usr.PinPolicy != null)
                        newUser.PinPolicy = usr.PinPolicy.ToString();

                    //PresencePolicy
                    if (usr.PresencePolicy != null)
                        newUser.PresencePolicy = usr.PresencePolicy.ToString();

                    //RegistrarPool
                    if (usr.RegistrarPool != null)
                        newUser.RegistrarPool = usr.RegistrarPool.ToString();

                    //SamAccountName
                    if (usr.SamAccountName != null)
                        newUser.SamAccountName = usr.SamAccountName.ToString();

                    //ThirdPartyVideoSystemPolicy
                    if (usr.ThirdPartyVideoSystemPolicy != null)
                        newUser.ThirdPartyVideoSystemPolicy = usr.ThirdPartyVideoSystemPolicy.ToString();

                    //UserServicesPolicy
                    if (usr.UserServicesPolicy != null)
                        newUser.UserServicesPolicy = usr.UserServicesPolicy.ToString();

                    //VoicePolicy
                    if (usr.VoicePolicy != null)
                        newUser.VoicePolicy = usr.VoicePolicy.ToString();

                    //VoiceRoutingPolicy
                    if (usr.VoiceRoutingPolicy != null)
                        newUser.VoiceRoutingPolicy = usr.VoiceRoutingPolicy.ToString();                           
                    
                    ProcessedUsers.Add(newUser);
                }
            }
            return ProcessedUsers;
        }


        /// <summary>
        /// Create a list of users in the format "Firstname Lastname (user@sipdomain.com)"
        /// </summary>
        /// <param name="csUsers"></param>
        public void CreateRgsAgentList(ICollection<PSObject> csUsers)
        {
            _csUsers = csUsers;
            RgsAgentList = new Dictionary<string, string>();

            foreach (dynamic user in _csUsers)
            {
                if (user != null)
                {
                    RgsAgentList.Add(user.Name + " (" + user.SipAddress + ")", user.SipAddress);
                }
                
            }
        }
    }
}
