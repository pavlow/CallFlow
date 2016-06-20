using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Management.Automation;
using System.Windows;
using System.Windows.Input;
using CallFlowManager.UI.Business;
using CallFlowManager.UI.Common;
using CallFlowManager.UI.ViewModels.Groups;
using Lync_WCF;
using System.Threading.Tasks;

namespace CallFlowManager.UI.ViewModels.Queues
{
    public class QueuesViewModel : PropertyChangedBase
    {
        public QueuesViewModel()
        {
            //Initialise
            _loadedQueues = new List<QueueViewModel>();

            CurrentQueue = new QueueViewModel();
            SelectedQueue = new QueueViewModel();
            SelectedGroup = new GroupViewModel();

            Destinations = Globals.QueueDestinations;
            QueueOverFlowCandidates = Globals.QueueOverFlowCandidates;

            Queues = new ObservableCollection<QueueViewModel>();
            Groups = new ObservableCollection<GroupViewModel>();

            QAgentCallGroupsAddCommand = new RelayCommand(_ => QAgentCallGroupsAdd());
            QAgentCallGroupsRemoveCommand = new RelayCommand(_ => QAgentCallGroupsRemove());
            QAgentCallGroupsUpCommand = new RelayCommand(_ => QAgentCallGroupsUp());
            QAgentCallGroupsDnCommand = new RelayCommand(_ => QAgentCallGroupsDn());

            //BACKGROUND WORKERS - Initialise https://visualstudiomagazine.com/articles/2010/11/18/multithreading-in-winforms.aspx
            //_backgroundWorker = new BackgroundWorker();
            //_backgroundWorker.DoWork += DoWork_LoadQueues;
            //_backgroundWorker.RunWorkerCompleted += ChangeStatus;

            //LoadCommand = new RelayCommand(_ => { if (!_backgroundWorker.IsBusy)_backgroundWorker.RunWorkerAsync(); });
            LoadCommand = new RelayCommand(_ => Load());

            //NEED TO ADD BACKGROUND WORKER
            CreateCommand = new RelayCommand(_ => Update());

            ClearCommand = new RelayCommand(_ => Clear());
            DeleteCommand = new RelayCommand(_ => Delete());
            
            //Initiate Queue Get
            //LoadCommand.Execute(null);
            
            
            //Populate shared data
            //SipDomains = _dataService.SipDomains.ToList();
            //UserAgents = _dataService.UsersList;
            //UserAgents = _dataService.UsersList as Dictionary<string, string>;

            //Subscribes to the OnGroupsUpdated event and updates the Queue groups drop down
            _dataService.OnGroupsUpdated += delegate(EventArgs args)
            {
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    Groups.Clear();
                    foreach (var group in _dataService.GroupsData.ToList().OrderBy(a => a.Name))
                    {
                        Groups.Add(group);
                    };
                });
            };
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


        public List<string> SipDomains
        {
            get { return _sipDomains; }
            set
            {
                if (_sipDomains != value)
                {
                    _sipDomains = value;
                    OnPropertyChanged("SipDomains");
                }
            }
        }

        /// <summary>
        /// Runs the load task when the user selects the "Refresh" button. The load task will query SfB data.
        /// </summary>
        /// <returns></returns>
        public async Task Load()
        {
            await Task.Run(() =>
            {
                ////This used to live in the LoadGroups method on its own
                IsLoading = true;
                StatusBar = "Loading queues, please wait...";
                _loadedQueues.Clear();
                //_dataService.LoadQueues();
                _dataService.GetQueues(true, true);

                if (_dataService.QueueData != null)
                {
                    _loadedQueues = _dataService.QueueData.OrderBy(a => a.Name).ToList();
                }
                else
                {
                    //Error loading Queues...
                }

                //SHARED
                //_dataService.LoadSipDomains(); 
                //Call as dependancy of LoadQueues
                SipDomains = _dataService.SipDomains;

                //Move to a property::
                Pools = _dataService.Pools;
                OnPropertyChanged("Pools");

                //QueueAgentGroups = _dataService.QueueAgentGroups;
                //OnPropertyChanged("QueueAgentGroups");
                //LoadQueues();
            });

            ////This used to live in the ChangeStatus method which was executed after the background worker completed
            ////Cannot run this within above thread!
            Queues.Clear();
            foreach (var queueModel in _loadedQueues)
            {
                QueueViewModel flow = queueModel;
                flow.QueuesViewModel = this;
                queueModel.QueuesViewModel = this;
                Application.Current.Dispatcher.Invoke((Action)(() => Queues.Add(flow)));
            }

            SipDomains = _dataService.SipDomains;

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
            if (SelectedQueue == null)
            {
                StatusBar = "Select a group to update and try again";
            }

            if (SelectedGroup != null)
            {
                IsLoading = true;
                StatusBar = "Updating queue, please wait...";
                await Task.Run(() =>
                {
                    var set = new QueuesSetService();
                    var queueObj = set.PrepareSetCsRgsQueue(CurrentQueue.Description, CurrentQueue.Group, CurrentQueue.Groups, CurrentQueue.Id, CurrentQueue.Name, 
                        CurrentQueue.OverFlow, CurrentQueue.OverFlowDestination, CurrentQueue.OverFlowSipDomain, CurrentQueue.OverFlowUri, CurrentQueue.OverflowOn,
                        CurrentQueue.OverFlowCandidate, CurrentQueue.OverFlowQueue, CurrentQueue.Timeout, CurrentQueue.TimeoutDestination, CurrentQueue.TimeoutOn, 
                        CurrentQueue.TimeoutSipDomain, CurrentQueue.TimeoutUri, CurrentQueue.TimeoutQueue, CurrentQueue.OwnerPool);

                    var psQueries = new PsQueries();
                    psQueries.SetCsRgsQueue(queueObj);
                });
                StatusBar = "";
                IsLoading = false;
                this.Load();
            }
        }

        public void Clear()
        {
            CurrentQueue = new QueueViewModel();
            SelectedQueue = null;
            SelectedQueue = new QueueViewModel();
        }

        public void Delete()
        {

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

        public QueueViewModel SelectedQueue
        {
            get { return _selectedQueue; }
            set
            {
                if (_selectedQueue != value)//&& value != null
                {
                    _selectedQueue = value;

                    if (_selectedQueue != null)
                    {
                        _selectedQueue.QueuesViewModel = this; 
                        CurrentQueue = (QueueViewModel)value.Clone();
                    }
                    OnPropertyChanged("SelectedQueue");
                }
            }
        }

        public GroupViewModel SelectedGroup
        {
            get { return _selectedGroup; }
            set
            {
                if (_selectedGroup != value && value != null)
                {
                    _selectedGroup = value;
                    OnPropertyChanged("SelectedGroup");
                }
            }
        }

        public QueueGroupViewModel SelectedAgent
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

        public QueueViewModel CurrentQueue
        {
            get { return _currentQueue; }
            set
            {
                if (_currentQueue != value) //&& value != null
                {
                    _currentQueue = value;
                    _currentQueue.QueuesViewModel = this;
                    OnPropertyChanged("CurrentQueue");
                }
            }
        }

        private void QAgentCallGroupsAdd()
        {
            if (SelectedGroup.Name != null)
            {
                if (!CurrentQueue.Groups.Any(p => p.Name.Contains(SelectedGroup.Name)))
                {
                    CurrentQueue.Groups.Add(new QueueGroupViewModel(SelectedGroup.Name, SelectedGroup.Timeout.ToString(),
                        SelectedGroup.ParticipationPolicy, SelectedGroup.RoutingMethod));
                }
            }
        }

        private void QAgentCallGroupsRemove()
        {
            CurrentQueue.Groups.Remove(SelectedAgent);
            if (CurrentQueue.Groups.Count > 0)
            {
                SelectedAgent = CurrentQueue.Groups.First();
            }
        }

        private void QAgentCallGroupsUp()
        {
            var indexItem = CurrentQueue.Groups.IndexOf(SelectedAgent);
            if (indexItem > 0)
            {
                var temp = CurrentQueue.Groups.ElementAt(indexItem);
                CurrentQueue.Groups.Remove(temp);
                CurrentQueue.Groups.Insert(indexItem - 1, temp);
                SelectedAgent = CurrentQueue.Groups.ElementAt(indexItem - 1);
                SelectedIndex = indexItem - 1;
            }
        }

        private void QAgentCallGroupsDn()
        {
            var indexItem = CurrentQueue.Groups.IndexOf(SelectedAgent);
            if (indexItem < CurrentQueue.Groups.Count - 1)
            {
                var temp = CurrentQueue.Groups.ElementAt(indexItem);
                CurrentQueue.Groups.Remove(temp);
                CurrentQueue.Groups.Insert(indexItem + 1, temp);
                SelectedAgent = CurrentQueue.Groups.ElementAt(indexItem + 1);
                SelectedIndex = indexItem + 1;
            }
        }

        //Class wide variables
        private string _name;
        private List<string> _sipDomains;
        private IList<string> _queueAgentGroups;
        private QueueViewModel _selectedQueue;
        private GroupViewModel _selectedGroup;
        private QueueViewModel _currentQueue;
        private QueueGroupViewModel _selectedAgent;
        private int _selectedIndex;

        private readonly IDataService _dataService = DataServiceFactory.Get();
        private BackgroundWorker _backgroundWorker;
        private bool _isLoading;
        private bool _isEnabled;
        private string _statusBar;
        private List<QueueViewModel> _loadedQueues;

        public ICommand LoadCommand { get; private set; }
        public ICommand CreateCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand QAgentCallGroupsAddCommand { get; private set; }
        public ICommand QAgentCallGroupsRemoveCommand { get; private set; }
        public ICommand QAgentCallGroupsUpCommand { get; private set; }
        public ICommand QAgentCallGroupsDnCommand { get; private set; }
        public ObservableCollection<QueueViewModel> Queues { get; set; }
        public ObservableCollection<GroupViewModel> Groups { get; set; }
        public Dictionary<string, string> Destinations { get; private set; }
        public Dictionary<string, string> QueueOverFlowCandidates { get; private set; }
        public Dictionary<string, string> UserAgents { get; private set; }
        public IEnumerable<string> Pools { get; private set; }

    }
}