using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using CallFlowManager.UI.ViewModels.Users;

namespace CallFlowManager.UI.Business
{
    public class UserPoliciesGetService
    {
        private ICollection<PSObject> _csUserPoicies { get; set; }
        public List<UserPoliciesViewModel> ProcessedUserPolicies;
        private UserPoliciesViewModel newUserPolicies = new UserPoliciesViewModel();
        //private ObservableCollection<string> _col;

        public UserPoliciesViewModel ProcessPsUserPolicies(ICollection<PSObject> csUserPoicies)
        {
            _csUserPoicies = csUserPoicies;
            ProcessedUserPolicies = new List<UserPoliciesViewModel>();

            //Add <Automatic> option to all drop downs
            string auto = "<Automatic>";
            newUserPolicies.VoicePolicy.Add(auto);
            newUserPolicies.VoiceRoutingPolicy.Add(auto);
            newUserPolicies.ConferencingPolicy.Add(auto);
            newUserPolicies.PresencePolicy.Add(auto);
            newUserPolicies.DialPlan.Add(auto);
            newUserPolicies.LocationPolicy.Add(auto);
            newUserPolicies.ClientPolicy.Add(auto);
            newUserPolicies.ClientVersionPolicy.Add(auto);
            newUserPolicies.ArchivingPolicy.Add(auto);
            newUserPolicies.PinPolicy.Add(auto);
            newUserPolicies.ExternalAccessPolicy.Add(auto);
            newUserPolicies.MobilityPolicy.Add(auto);
            newUserPolicies.PersistentChatPolicy.Add(auto);
            newUserPolicies.UserServicesPolicy.Add(auto);
            newUserPolicies.CallViaWorkPolicy.Add(auto);
            newUserPolicies.HostedVoicemailPolicy.Add(auto);
            newUserPolicies.RegistrarPool.Add(auto);
            newUserPolicies.ExchangeArchivingPolicy.Add(auto);

            //Populate static options
            newUserPolicies.ExchangeArchivingPolicy.Add("Uninitialized");
            newUserPolicies.ExchangeArchivingPolicy.Add("UseLyncArchivingPolicy");
            newUserPolicies.ExchangeArchivingPolicy.Add("ArchivingToExchange");
            newUserPolicies.ExchangeArchivingPolicy.Add("NoArchiving");

            //Populate options from PowerShell Query 
            foreach (dynamic dyn in _csUserPoicies)
            {                
                if (dyn != null)
                {
                    switch ((string)dyn.PolicyType)
                    {
                        case "VoicePolicy":
                            newUserPolicies.VoicePolicy.Add(dyn.Name);
                            break;

                        case "VoiceRoutingPolicy":
                             newUserPolicies.VoiceRoutingPolicy.Add(dyn.Name);
                             break;

                        case "ConferencingPolicy":
                             newUserPolicies.ConferencingPolicy.Add(dyn.Name);
                             break;

                        case "PresencePolicy":
                             newUserPolicies.PresencePolicy.Add(dyn.Name);
                             break;

                        case "DialPlan":
                             newUserPolicies.DialPlan.Add(dyn.Name);
                             break;

                        case "LocationPolicy":
                             newUserPolicies.LocationPolicy.Add(dyn.Name);
                             break;

                        case "ClientPolicy":
                             newUserPolicies.ClientPolicy.Add(dyn.Name);
                             break;

                        case "ClientVersionPolicy":
                             newUserPolicies.ClientVersionPolicy.Add(dyn.Name);
                             break;

                        case "ArchivingPolicy":
                             newUserPolicies.ArchivingPolicy.Add(dyn.Name);
                             break;

                        case "PinPolicy":
                             newUserPolicies.PinPolicy.Add(dyn.Name);
                             break;

                        case "ExternalAccessPolicy":
                             newUserPolicies.ExternalAccessPolicy.Add(dyn.Name);
                             break;

                        case "MobilityPolicy":
                             newUserPolicies.MobilityPolicy.Add(dyn.Name);
                             break;

                        case "PersistentChatPolicy":
                             newUserPolicies.PersistentChatPolicy.Add(dyn.Name);
                             break;

                        case "UserServicesPolicy":
                             newUserPolicies.UserServicesPolicy.Add(dyn.Name);
                             break;

                        case "CallViaWorkPolicy":
                             newUserPolicies.CallViaWorkPolicy.Add(dyn.Name);
                             break;

                        case "HostedVoicemailPolicy":
                             newUserPolicies.HostedVoicemailPolicy.Add(dyn.Name);
                             break;

                        case "RegistrarPool":
                             newUserPolicies.RegistrarPool.Add(dyn.Name);
                             break;

                        //case "ExchangeArchivingPolicy":
                        //     newUserPolicies.ExchangeArchivingPolicy.Add(dyn.Name);
                        //     break;

                    }
                    

                    //Andrews Code - commenting out as the switch above replaces it
                    //if (dyn.PolicyType == "VoicePolicy")
                    //{
                    //    var _col = new ObservableCollection<string>();
                    //    _col.Add(dyn.Name);
                    //    newUserPolicies.VoicePolicy.Add(dyn.Name);// = _col; //.Add(dyn.Name);                      
                    //}



                }
            }
            //ProcessedUserPolicies.Add(newUserPolicies);
            //return ProcessedUserPolicies;
            return newUserPolicies;
        }
    }
}
//VoicePolicy
//VoiceRoutingPolicy
//ConferencingPolicy
//PresencePolicy
//DialPlan
//LocationPolicy
//ClientPolicy
//ClientVersionPolicy
//ArchivingPolicy
//PinPolicy
//ExternalAccessPolicy
//MobilityPolicy
//PersistentChatPolicy
//UserServicesPolicy
//CallViaWorkPolicy
//HostedVoicemailPolicy
//RegistrarPool
