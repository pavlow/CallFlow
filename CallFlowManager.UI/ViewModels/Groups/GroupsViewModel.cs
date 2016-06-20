using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using CallFlowManager.UI.Business;
using CallFlowManager.UI.Common;
using System.Threading.Tasks;
using Logging;

namespace CallFlowManager.UI.ViewModels.Groups
{
    public class GroupsViewModel : PropertyChangedBase
    {
        private static readonly NLogger Logger = LoggerFactory.Instance.GetCurrentClassLogger() as NLogger;

        //Class wide variables
        //Regex e.g. User Three (sip:u3@ucgeek.nz)
        private static readonly Regex RegexAgent = new Regex(@"^(?<name>.*)\((?<sip>.*)\).*$");
        private string _name;
        private string _selectedUser;
        private int _selectedIndex;
        private GroupViewModel _selectedGroup;
        private AgentGroupViewModel _selectedAgent;
        private GroupViewModel _currentGroup;
        private readonly IDataService _dataService = DataServiceFactory.Get();
        private readonly PsQueries _psQ;
        private readonly SharedPsService _sharedPsService;
        private readonly BackgroundWorker _backgroundWorker;
        private List<GroupViewModel> _loadedGroups;
        private bool _isLoading;
        private bool _isEnabled;
        private string _statusBar;

        public GroupsViewModel()
        {
            Logger.Trace("Started loading Groups");
            _backgroundWorker = new BackgroundWorker();
            _loadedGroups = new List<GroupViewModel>();
            CurrentGroup = new GroupViewModel();
            SelectedGroup = new GroupViewModel();

            _psQ = new PsQueries();
            _sharedPsService = new SharedPsService();

            RoutingMethods = Globals.GrpRoutingMethods;
            LoadCommand = new RelayCommand(_ => LoadGroups());
            UpdateCommand = new RelayCommand(_ => Update());
            ClearCommand = new RelayCommand(_ => Clear());
            DeleteCommand = new RelayCommand(_ => Delete());
            GAgentCallGroupsAddCommand = new RelayCommand(_ => GAgentCallGroupsAdd());
            GAgentCallGroupsRemoveCommand = new RelayCommand(_ => GAgentCallGroupsRemove());
            GAgentCallGroupsUpCommand = new RelayCommand(_ => GAgentCallGroupsUp());
            GAgentCallGroupsDnCommand = new RelayCommand(_ => GAgentCallGroupsDn());
            Groups = new ObservableCollection<GroupViewModel>();
            //UserAgents = _dataService.UsersList as Dictionary<string, string>;
        }

        public Dictionary<string, string> RoutingMethods { get; private set; }
        public ICommand LoadCommand { get; private set; }
        public ICommand UpdateCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand GAgentCallGroupsAddCommand { get; private set; }
        public ICommand GAgentCallGroupsRemoveCommand { get; private set; }
        public ICommand GAgentCallGroupsUpCommand { get; private set; }
        public ICommand GAgentCallGroupsDnCommand { get; private set; }
        public ObservableCollection<GroupViewModel> Groups { get; private set; }
        public Dictionary<string, string> UserAgents { get; private set; }
        public IEnumerable<string> Pools { get; private set; }

        /// <summary>
        /// Value to display on the status bar
        /// </summary>
        public string StatusBar
        {
            get { return _statusBar; }
            set
            {
                if (_statusBar != value)
                {
                    _statusBar = value;
                    OnPropertyChanged("StatusBar");
                }
            }
        }

        /// <summary>
        /// True when the application is processing data
        /// </summary>
        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;

                    if (value)
                        IsEnabled = false;
                    if (!value)
                        IsEnabled = true;

                    OnPropertyChanged("IsLoading");
                }
            }
        }

        /// <summary>
        /// Binding to tell WPF controls whether they should be disabled or not
        /// </summary>
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged("IsEnabled");
                }
            }
        }


        /// <summary>
        /// Runs the load task when the user selects the "Refresh" button. The load task will query SfB data.
        /// </summary>
        /// <returns></returns>
        public async Task LoadGroups()
        {
            Logger.Trace("Load/Update Groups");
            var prevGroup = string.Empty;
            if (SelectedGroup != null)
            {
                prevGroup = SelectedGroup.Name;
            }

            await Task.Run(() =>
            {
                ////This used to live in the LoadGroups method on its own
                IsLoading = true;
                StatusBar = "Loading groups, please wait...";
                _loadedGroups.Clear();
                //_dataService.LoadRgsAgents(); 
                //Move as a dependency of LoadGroups:
                //_dataService.LoadGroups();
                _dataService.GetGroups(true, true);

                _loadedGroups = _dataService.GroupsData.ToList();
                //Move to property:
                UserAgents = _dataService.RgsAgents;
                OnPropertyChanged("UserAgents");

                //Move to a property::
                Pools = _dataService.Pools;
                //Pools = _sharedPsService.ProcessRegistrarPools(_psQ.GetRegistrarPools());
                OnPropertyChanged("Pools");
            });

            ////This used to live in the ChangeStatus method which was executed after the background worker completed
            ////Cannot run this within above thread!
            Groups.Clear();
            foreach (var groupModel in _loadedGroups.OrderBy(a => a.Name))
            {
                GroupViewModel flow = groupModel;
                if (flow.Name.Equals(prevGroup))
                {
                    SelectedGroup = flow;
                }

                Application.Current.Dispatcher.Invoke((Action)(() => Groups.Add(flow)));
            }
            StatusBar = "";
            IsLoading = false;
            CurrentGroup = _currentGroup;
        }

        /// <summary>
        /// Runs the update task when the user selects the "Create/Update" button. The update task will commit changes to SfB.
        /// </summary>
        /// <returns></returns>
        public async Task Update()
        {
            if (SelectedGroup == null)
            {
                StatusBar = "Select a group to update and try again";
            }

            if (SelectedGroup != null)
            {
                var prevGroup = SelectedGroup.Name;
                IsLoading = true;
                StatusBar = "Updating group, please wait...";
                await Task.Run(() =>
                {
                    var set = new GroupsSetService();
                    var grpObj = set.PrepareSetCsRgsGroup(CurrentGroup.Agents, CurrentGroup.Description, CurrentGroup.DistributionGroup, CurrentGroup.Identity,
                        CurrentGroup.IsDistributionGroup, CurrentGroup.IsGroupAgentSignIn, CurrentGroup.IsGroupAgents, CurrentGroup.Name, CurrentGroup.OwnerPool,
                        CurrentGroup.ParticipationPolicy, CurrentGroup.RoutingMethod, CurrentGroup.Timeout);

                    var psQueries = new PsQueries();
                    psQueries.SetCsRgsAgentGroup(grpObj);
                });
                StatusBar = "";
                IsLoading = false;
                LoadGroups();

                foreach (var group in _loadedGroups)
                {
                    if (group.Name.Equals(prevGroup))
                    {
                        SelectedGroup = group;
                    }
                }
            }
        }

        public void Clear()
        {
            CurrentGroup = new GroupViewModel();
            SelectedGroup = null;
            SelectedGroup = new GroupViewModel();
        }

        public void Delete()
        {

        }

        //private void CreateGroup()
        //{
        //    List<string> AgentsByUri = new List<string>();
        //    string AgentsCSV = "";
        //    foreach (var Agent in CurrentGroup.Agents.OrderBy(a => a.Name))
        //    {
        //        AgentsCSV += Agent.SipAddress.Trim() + ",";
        //    }
        //    AgentsCSV = AgentsCSV.TrimEnd(',');

        //    string NewRoutingGroup = "";
        //    if (!string.IsNullOrEmpty(CurrentGroup.RoutingMethod))
        //    {
        //        var tidyUp = CurrentGroup.RoutingMethod;
        //        tidyUp = tidyUp.TrimStart('[');
        //        tidyUp = tidyUp.TrimEnd(']');
        //        NewRoutingGroup = tidyUp.Split(',')[0].Trim();
        //    }

        //    //TODO: validation before going to powershell

        //    var LyncService = new Lync_WCF.LyncServerManager();

        //    LyncService.SetCsRgsAgentGroup(CurrentGroup.Identity, CurrentGroup.Name, CurrentGroup.Description ?? "", CurrentGroup.ParticipationPolicy,
        //        CurrentGroup.Timeout.ToString(), NewRoutingGroup, CurrentGroup.DistributionGroup, CurrentGroup.Owner, AgentsCSV);

        //    //TODO: need to get result back from PsFactory and display in UI
        //    //Prob need to change from VOID

        //}

        
        public GroupViewModel SelectedGroup
        {
            get { return _selectedGroup; }
            set
            {
                if (_selectedGroup != value)//&& value != null
                {
                    _selectedGroup = value;

                    if (_selectedGroup != null)
                    {
                        CurrentGroup = (GroupViewModel) value.Clone();
                    }
                    OnPropertyChanged("SelectedGroup");
                }
            }
        }

        public GroupViewModel CurrentGroup
        {
            get { return _currentGroup; }
            set
            {
                if (_currentGroup != value && value != null)
                {
                    _currentGroup = value;
                    OnPropertyChanged("CurrentGroup");
                }
            }
        }

        public string SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (_selectedUser != value && value != null)
                {
                    _selectedUser = value;
                    OnPropertyChanged("SelectedUser");
                }
            }
        }

        public int SelectedIndex
        {
            get
            {
                return this._selectedIndex;
            }
            set
            {
                this._selectedIndex = value;
                OnPropertyChanged("SelectedIndex");
            }
        }

        public AgentGroupViewModel SelectedAgent
        {
            get { return _selectedAgent; }
            set
            {
                if (_selectedAgent != value && value != null)
                {
                    _selectedAgent = value;
                    OnPropertyChanged("SelectedAgent");
                }
            }
        }

        private void GAgentCallGroupsAdd()
        {
            string nameId = null;
            string sipId = null;
            Match match;
            if (SelectedUser != null)
            {
                match = RegexAgent.Match(SelectedUser);
                if (match.Success)
                {
                    nameId = match.Groups["name"].Value.Trim();
                    sipId = match.Groups["sip"].Value.Trim();
                }
                if (!CurrentGroup.Agents.Any(p => nameId != null && p.Name.Contains(nameId)))
                {
                    CurrentGroup.Agents.Add(new AgentGroupViewModel() { Name = nameId, SipAddress = sipId });
                }
            }
        }

        private void GAgentCallGroupsRemove()
        {
            CurrentGroup.Agents.Remove(SelectedAgent);
            if (CurrentGroup.Agents.Count > 0)
            {
                SelectedAgent = CurrentGroup.Agents.First();
            }
        }

        private void GAgentCallGroupsUp()
        {
            var indexItem = CurrentGroup.Agents.IndexOf(SelectedAgent);
            if (indexItem > 0)
            {
                var temp = CurrentGroup.Agents.ElementAt(indexItem);
                CurrentGroup.Agents.Remove(temp);
                CurrentGroup.Agents.Insert(indexItem - 1, temp);
                SelectedAgent = CurrentGroup.Agents.ElementAt(indexItem - 1);
                SelectedIndex = indexItem - 1;
            }
        }

        private void GAgentCallGroupsDn()
        {
            var indexItem = CurrentGroup.Agents.IndexOf(SelectedAgent);
            if (indexItem < CurrentGroup.Agents.Count - 1)
            {
                var temp = CurrentGroup.Agents.ElementAt(indexItem);
                CurrentGroup.Agents.Remove(temp);
                CurrentGroup.Agents.Insert(indexItem + 1, temp);
                SelectedAgent = CurrentGroup.Agents.ElementAt(indexItem + 1);
                SelectedIndex = indexItem + 1;
            }            
        }
        

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        
    }
}