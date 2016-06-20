using System.Collections.Generic;
using System.Management.Automation;
using CallFlowManager.UI.ViewModels.Users;

namespace CallFlowManager.UI.Business
{
    public class UsersServiceOLD
    {
        //private readonly PsQueries _dataRetriever;
        private ICollection<PSObject> _csUsers { get; set; }

        public Dictionary<string, string> RgsAgentList;

        public List<UserViewModel> ProcessedUsers;

        public UsersServiceOLD(PsQueries dataRetriever)
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
