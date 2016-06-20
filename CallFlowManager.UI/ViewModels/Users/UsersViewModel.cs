using CallFlowManager.UI.Business;
using CallFlowManager.UI.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CallFlowManager.UI.ViewModels.Users
{
    public class UsersViewModel : PropertyChangedBase, IDataErrorInfo
    {
//        private string _name;
        private UserViewModel _selectedUser;
        private UserViewModel _currentUser;
        private readonly IDataService _dataService = DataServiceFactory.Get();
        //private readonly BackgroundWorker _backgroundWorker;
        private List<UserViewModel> _loadedUsers;
        private UserPoliciesViewModel _loadedUserPolicies;

        private List<string> _UserPolicyTemplates;

        private bool _isLoading;
        private bool _isEnabled;
        private string _statusBar;

        private Dictionary<string, bool> validProperties;
        private bool allPropertiesValid = false;


        public ICommand LoadCommand { get; private set; }
        public ICommand CreateCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand TemplateSaveCommand { get; private set; }
        public ICommand TemplateDeleteCommand { get; private set; }
        public ICommand TemplateResetCommand { get; private set; }

        public ObservableCollection<UserViewModel> Users { get; private set; }
        public UserPoliciesViewModel UserPolicies { get; private set; }


        public UsersViewModel()
        {
            //_backgroundWorker = new BackgroundWorker();
            //_backgroundWorker.DoWork += LoadUsers;
            //_backgroundWorker.RunWorkerCompleted += ChangeStatus;


            _loadedUsers = new List<UserViewModel>();

            CurrentUser = new UserViewModel();
            SelectedUser = new UserViewModel();


            //LoadCommand = new RelayCommand(_ => { if (!_backgroundWorker.IsBusy)_backgroundWorker.RunWorkerAsync(); });
            //CreateCommand = new RelayCommand(_ => CreateUser());

            //Form button relay commands
            LoadCommand = new RelayCommand(_ => Load());
            CreateCommand = new RelayCommand(_ => Update());
            ClearCommand = new RelayCommand(_ => Clear());
            DeleteCommand = new RelayCommand(_ => Delete());

            TemplateSaveCommand = new RelayCommand(_ => TemplateSave());
            TemplateDeleteCommand = new RelayCommand(_ => TemplateDelete());
            TemplateResetCommand = new RelayCommand(_ => TemplateReset());

            Users = new ObservableCollection<UserViewModel>();
            //UserAgents = _dataService.UsersList as Dictionary<string, string>;

            this.validProperties = new Dictionary<string, bool>();
            this.validProperties.Add("DisplayName", false);
            this.validProperties.Add("SipAddress", false);
            this.validProperties.Add("LineUri", false);
        }

        public bool AllPropertiesValid
        {
            get { return allPropertiesValid; }
            set
            {
                if (allPropertiesValid != value)
                {
                    allPropertiesValid = value;
                    base.OnPropertyChanged("AllPropertiesValid");
                }
            }
        }

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

        //private void LoadUsers(object sender, DoWorkEventArgs e)
        //{
        //    IsLoading = true;

        //    _loadedUsers.Clear();
        //    _dataService.LoadUsers();
        //    //_loadedUsers = _dataService.UsersList.ToList();
        //    _loadedUsers = _dataService.UsersData.ToList();
        //    //OnPropertyChanged("UserAgents");
        //}

        //private void ChangeStatus(object sender, RunWorkerCompletedEventArgs e)
        //{
        //    IsLoading = false;
        //    Users.Clear();
        //    foreach (var user in _loadedUsers.OrderBy(a => a.DisplayName))
        //    {
        //        UserViewModel flow = user;
        //        Application.Current.Dispatcher.Invoke((Action)(() => Users.Add(flow)));
        //    }
        //}

        /// <summary>
        /// Runs the load task when the user selects the "Refresh" button. The load task will query SfB data.
        /// </summary>
        /// <returns></returns>
        public async Task Load()
        {
            await Task.Run(() =>
            {
                ////This used to live in the LoadUsers method on its own
                IsLoading = true;
                StatusBar = "Loading users and policies, please wait...";
                _loadedUsers.Clear();

                //_dataService.LoadUsers();
                //_dataService.LoadUserPolicies();
                _dataService.GetUsers(true, true);
                _dataService.GetUserPolicies(true, true);

                _loadedUsers = _dataService.UsersData.ToList();
                _loadedUserPolicies = _dataService.UserPoliciesData;

                //SHARED

                _UserPolicyTemplates = GetTemplatePolicies().Select(a => a.PolicyTemplateName).ToList(); ;


            });

            ////This used to live in the ChangeStatus method which was executed after the background worker completed
            ////Cannot run this within above thread!
            //Users:
            Users.Clear();
            foreach (var user in _loadedUsers.OrderBy(a => a.DisplayName))
            {
                UserViewModel flow = user;
                Application.Current.Dispatcher.Invoke((Action)(() => Users.Add(flow)));
            }

            //User Policies:
            UserPolicies = _loadedUserPolicies;
            OnPropertyChanged("UserPolicies");

            StatusBar = "";
            IsLoading = false;
            ////



        }

        /// <summary>
        /// Runs the update task when the user selects the "Create/Update" button. The update task will commit changes to SfB.
        /// </summary>
        /// <returns></returns>
        public async Task Update()
        {
            if (SelectedUser == null)
            {
                StatusBar = "Select a user to update and try again";
            }

            if (SelectedUser != null)
            {
                IsLoading = true;
                StatusBar = "Updating user, please wait...";
                await Task.Run(() =>
                {
                    //var set = new QueuesSetService();
                    //var queueObj = set.PrepareSetCsRgsQueue(CurrentQueue.Description, CurrentQueue.Group, CurrentQueue.Groups, CurrentQueue.Id, CurrentQueue.Name,
                    //    CurrentQueue.OverFlow, CurrentQueue.OverFlowDestination, CurrentQueue.OverFlowSipDomain, CurrentQueue.OverFlowUri, CurrentQueue.OverflowOn,
                    //    CurrentQueue.OverFlowCandidate, CurrentQueue.OverFlowQueue, CurrentQueue.Timeout, CurrentQueue.TimeoutDestination, CurrentQueue.TimeoutOn,
                    //    CurrentQueue.TimeoutSipDomain, CurrentQueue.TimeoutUri, CurrentQueue.TimeoutQueue, CurrentQueue.OwnerPool);

                    //var psQueries = new PsQueries();
                    //psQueries.SetCsRgsQueue(queueObj);
                });
                StatusBar = "";
                IsLoading = false;
                this.Load();
            }
        }

        public void Clear()
        {
            CurrentUser = new UserViewModel();
            SelectedUser = null;
            SelectedUser = new UserViewModel();
        }

        public void Delete()
        {

        }


        /// <summary>
        /// Button control - Save Template.
        /// </summary>
        /// <returns></returns>
        public async Task TemplateSave()
        {
            MessageBox.Show("You clicked Save");
        }


        /// <summary>
        /// Button control - Save Template.
        /// </summary>
        /// <returns></returns>
        public async Task TemplateDelete()
        {
            MessageBox.Show("You clicked delete");
        }


        /// <summary>
        /// Button control - Save Template.
        /// </summary>
        /// <returns></returns>
        public async Task TemplateReset()
        {
            MessageBox.Show("You clicked Reset");
        }




        public UserViewModel CurrentUser
        {
            get { return _currentUser; }
            set
            {
                if (_currentUser != value && value != null)
                {
                    _currentUser = value;
                    OnPropertyChanged("CurrentUser");
                }
            }
        }

        public UserViewModel SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (_selectedUser != value)//&& value != null
                {
                    _selectedUser = value;

                    if (_selectedUser != null)
                    {
                        CurrentUser = (UserViewModel)value.Clone();
                    }
                    OnPropertyChanged("SelectedUser");
                }
            }
        }

        private void CreateUser()
        {


            //TODO: validation before going to powershell

            var LyncService = new Lync_WCF.LyncServerManager();

            //LyncService.SetCsRgsAgentGroup(CurrentGroup.Identity, CurrentGroup.Name, CurrentGroup.Description ?? "", CurrentGroup.ParticipationPolicy,
            //    CurrentGroup.Timeout.ToString(), NewRoutingGroup, CurrentGroup.DistributionGroup, CurrentGroup.Owner, AgentsCSV);

            //TODO: need to get result back from PsFactory and display in UI
            //Prob need to change from VOID

        }

        private List<Lync_WCF.LyncPolicyTemplate> GetTemplatePolicies()
        {
            var LyncService = new Lync_WCF.LyncPolicyManager();
            return LyncService.GetPolicyTemplates();
        }

        #region IDataErrorInfo members

        //public string this[string columnName]
        //{
        //    get
        //    {
        //        string error = String.Empty;
        //        switch (columnName)
        //        {
        //            case "CurrentUser.DisplayName":
        //                if (CurrentUser.DisplayName.Contains("AAA"))
        //                {
        //                    error = " --- AAA --- ";
        //                }
        //                break;
        //            case "CurrentUser.SipAddress":
        //                if (CurrentUser.SipAddress.Contains("AAA"))
        //                {
        //                    error = " --- AAA --- ";
        //                }
        //                break;
        //            case "CurrentUser.LineUri":
        //                if (CurrentUser.LineUri.Contains("AAA"))
        //                {
        //                    error = " --- AAA --- ";
        //                }
        //                break;
        //        }
        //        return error;
        //    }
        //}

        public string this[string propertyName]
        {
            get
            {
                string error = (CurrentUser as IDataErrorInfo)[propertyName];
                validProperties[propertyName] = String.IsNullOrEmpty(error) ? true : false;
                ValidateProperties();
                CommandManager.InvalidateRequerySuggested();
                return error;
            }
        }

        public string Error
        {
            get { return (CurrentUser as IDataErrorInfo).Error; }
        }

        #endregion


        private void ValidateProperties()
        {
            foreach (bool isValid in validProperties.Values)
            {
                if (!isValid)
                {
                    this.AllPropertiesValid = false;
                    return;
                }
            }
            this.AllPropertiesValid = true;
        }

    }
}
