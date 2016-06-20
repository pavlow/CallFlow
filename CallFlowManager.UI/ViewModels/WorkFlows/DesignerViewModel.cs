using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CallFlowManager.UI.Business;
using CallFlowManager.UI.Common;
using CallFlowManager.UI.ViewModels.BusinessHours;
using CallFlowManager.UI.ViewModels.Holidays;
using CallFlowManager.UI.ViewModels.Queues;
using System.Threading.Tasks;
using CallFlowManager.UI.ViewModels.Users;
using CallFlowManager.UI.Models;
using NLog;

namespace CallFlowManager.UI.ViewModels.WorkFlows
{
    public class DesignerViewModel : PropertyChangedBase, IDataErrorInfo
    {
        # region Constructor
        public DesignerViewModel()
        {
            //Initialise
            _loadedWorkflows = new List<WorkFlowViewModel>();

            //_backgroundWorker = new BackgroundWorker();
            //_backgroundWorker.DoWork += LoadWorkFlows;
            //_backgroundWorker.RunWorkerCompleted += ChangeStatus;
            //_workFlowsLoaded = new List<WorkFlowViewModel>();
            Languages = Globals.Languages;
            TimeZones = Globals.TimeZones;
            Destinations = Globals.WfDestinations;
            //CurrentWorkFlow = new WorkFlowViewModel(this);
            //SelectedWorkFlow = new WorkFlowViewModel(this);

            //Moved to CurrentWorkflowband SelectedWorkflow : Creates a default CurrentWorkflow for cases when none is selected by the user and a new one is to be created. The DesignerViewModel property is set to ensure Ivr Options can be removed succesfully
            CurrentWorkFlow = new WorkFlowViewModel();
            //CurrentWorkFlow.DesignerViewModel = this;
            //_currentWorkFlow = new WorkFlowViewModel();
            //_currentWorkFlow.DesignerViewModel = this;

            SelectedWorkFlow = new WorkFlowViewModel();
            //SelectedWorkFlow.DesignerViewModel = this;

            //_selectedWorkFlow = new WorkFlowViewModel();
            //_selectedWorkFlow.DesignerViewModel = this;

            //LoadCommand = new RelayCommand(_ => { if (!_backgroundWorker.IsBusy) _backgroundWorker.RunWorkerAsync(); });
            LoadCommand = new RelayCommand(_ => Load());
            CreateCommand = new RelayCommand(_ => Update());
            ClearCommand = new RelayCommand(_ => Clear());
            DeleteCommand = new RelayCommand(_ => Delete());
            DeactivateCommand = new RelayCommand(_ => Deactivate());
            //            ClearMessageCommand = new RelayCommand(_ => ClearMessage());

            //AudioIvrSelectorCommand = new RelayCommand(_ => { MessageBox.Show("Test!"); });
            //AudioWelcomeSelectorCommand = new RelayCommand(_ => AudioWelcomeSelector());
            ////AudioWelcomeSelectorCommand = new RelayCommand<object>(CommandWithAParameter);

            //AudioAfterHoursSelectorCommand = new RelayCommand(_ => AudioAfterHoursSelector());
            //AudioHolidaysSelectorCommand = new RelayCommand(_ => AudioHolidaysSelector());
            //AudioIvrSelectorCommand = new RelayCommand(_ => AudioIvrSelector());
            //AudioHoldMusicSelectorCommand = new RelayCommand(_ => AudioHoldMusicSelector());

            WorkFlows = new ObservableCollection<WorkFlowViewModel>();
            //SipDomains = _dataService.SipDomains;
            Queues = new ObservableCollection<QueueViewModel>();

            NumbersFiltered = new ObservableCollection<Number>();
            UsersFiltered = new ObservableCollection<UserViewModel>();
            QueuesFiltered = new ObservableCollection<QueueViewModel>();
            WfFiltered = new ObservableCollection<WorkFlowViewModel>();
            LogCollection = new ObservableCollection<LogEventInfo>();


            RemoveSearchCommand = new RelayCommand(_ => RemoveSearch());

            //            VisibilityLog = "Collapsed";
            //            _logMessagePs = string.Empty;

            _dataService.OnQueuesUpdated += delegate(EventArgs args)
            {
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    Queues.Clear();
                    if (_dataService.QueueData != null)
                    {
                        foreach (var queue in _dataService.QueueData.ToList().OrderBy(a => a.Name))
                        {
                            Queues.Add(queue);
                        }

                    }
                    else
                    {
                        //Error loading Queues...
                    }

                    OnPropertyChanged("CurrentWorkFlow");
                });

            };


            BusinessHoursGroups = new ObservableCollection<BusinessHourGroupViewModel>();
            _dataService.OnBusinessHoursUpdated += delegate(EventArgs args)
            {
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    BusinessHoursGroups.Clear();
                    if (_dataService.BusinessHoursGroups != null)
                    {
                        if (_dataService.BusinessHoursGroups != null)
                            foreach (var @group in _dataService.BusinessHoursGroups.OrderBy(a => a.Name))
                            {

                                BusinessHoursGroups.Add(new BusinessHourGroupViewModel(@group));
                            }
                    }
                    else
                    {
                        //Error loading bus hours...
                    }

                });
            };

            HolidaysGroups = new ObservableCollection<HolidayGroupViewModel>();
            _dataService.OnHolidaysUpdated += delegate(EventArgs args)
            {
                App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
                {
                    HolidaysGroups.Clear();
                    if (_dataService.HolidayGroups != null)
                    {
                        if (_dataService.HolidayGroups != null)
                            foreach (var @group in _dataService.HolidayGroups.OrderBy(a => a.Name))
                            {

                                HolidaysGroups.Add(new HolidayGroupViewModel(@group));
                            }
                        ;
                    }
                    else
                    {
                        //Error loading holidays...
                    }

                });
            };
        }

        # endregion Constructor

        public static ObservableCollection<LogEventInfo> LogCollection { get; set; }

        ///// <summary>
        ///// Value to display on the LogWindow
        ///// </summary>
        //public string VisibilityLog
        //{
        //    get { return _visibilityLog; }
        //    set
        //    {
        //        if (_visibilityLog != value)
        //        {
        //            _visibilityLog = value;
        //            OnPropertyChanged("VisibilityLog");
        //        }
        //    }
        //}


        ///// <summary>
        ///// Text to display on the LogWindow
        ///// </summary>
        //public string LogMessagePs
        //{
        //    get { return _logMessagePs; }
        //    set
        //    {
        //        if (_logMessagePs != value)
        //        {
        //            _logMessagePs = value;
        //            VisibilityLog = "Visible";
        //            OnPropertyChanged("LogMessagePs");
        //            OnPropertyChanged("VisibilityLog");
        //        }
        //    }
        //}

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
                    {
                        IsEnabled = false;
                    }

                    if (!value)
                    {
                        IsEnabled = true;
                    }

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


        ////BusinessHoursGroupsFiltered = BusinessHoursGroups.Where(w => w.OwnerPool.Equals(CurrentWorkFlow.OwnerPool));

        //public IEnumerable<BusinessHourGroupViewModel> BusinessHoursGroupsFiltered
        //{
        //    get { return _businessHoursGroupsFiltered; }
        //    set
        //    {
        //        if (_businessHoursGroupsFiltered != value)
        //        {
        //            _businessHoursGroupsFiltered = value;
        //            OnPropertyChanged("BusinessHoursGroupsFiltered");
        //        }
        //    }
        //}


        /// <summary>
        /// Runs the load task when the user selects the "Refresh" button. The load task will query SfB data.
        /// </summary>
        /// <returns></returns>
        public async Task Load()
        {
            string prevWorkFlowName = CurrentWorkFlow.Name;
            await Task.Run(() =>
            {
                ////This used to live in the LoadWorkflows method on its own
                IsLoading = true;
                StatusBar = "Loading workflows, please wait...";

                _loadedWorkflows.Clear();
                //_dataService.LoadWorkFlows(this);
                _dataService.GetWorkflows(this, true, true);
                _loadedWorkflows = _dataService.WorkFlowsData;

                //Not required as Workflows query will not complete until all dependacies are met
                //while (!Queues.Any())
                //{
                //    System.Threading.Thread.Sleep(TimeSpan.FromSeconds(10));
                //}

                //if (_dataService.WorkFlows == null)
                //{
                //    return;
                //}
            });

            ////This used to live in the ChangeStatus method which was executed after the background worker completed
            ////Cannot run this within above thread!
            WorkFlows.Clear();
            foreach (var workFlow in _loadedWorkflows.OrderBy(a => a.Name))
            {
                WorkFlowViewModel flow = workFlow;
                //workFlow.DesignerViewModel = this;
                if (workFlow.Name.Equals(prevWorkFlowName))
                {
                    SelectedWorkFlow = workFlow;
                }
                flow.DesignerViewModel = this;
                Application.Current.Dispatcher.Invoke((Action)(() => WorkFlows.Add(flow)));
            }

            SipDomains = _dataService.SipDomains.ToList();
            OnPropertyChanged("SipDomains");
            OnPropertyChanged("SelectedWorkFlow");

            //Move to a property::
            Pools = _dataService.Pools;
            OnPropertyChanged("Pools");

            StatusBar = string.Empty;
            IsLoading = false;
            ////

        }


        /// <summary>
        /// Runs the update task when the user selects the "Create/Update" button. The update task will commit changes to SfB.
        /// </summary>
        /// <returns></returns>
        public async Task Update()
        {
            if (SelectedWorkFlow == null)
            {
                StatusBar = "Select a call flow to update and try again";
            }

            if (SelectedWorkFlow != null)
            {
                IsLoading = true;
                StatusBar = "Updating call flow, please wait...";

                //if (CurrentWorkFlow == null)
                //{
                //    CurrentWorkFlow = new WorkFlowViewModel();
                //}

                await Task.Run(() =>
                {
                    var psQueries = new PsQueries();
                    //psQueries.SetCsRgsWf(wfObj);
                    psQueries.SetCsRgsWf(CurrentWorkFlow);
                });
                StatusBar = string.Empty;
                IsLoading = false;
 //               var clone = (WorkFlowViewModel)CurrentWorkFlow.Clone();
 //               WorkFlows.Add(clone);
                SelectedWorkFlow = CurrentWorkFlow;
                this.Load();
            }

            //Unsure what this does:
            //if (!CurrentWorkFlow.Name.Equals(SelectedWorkFlow.Name))
            //{
            //    WorkFlows.Add(CurrentWorkFlow);
            //}
            //else
            //{
            //    SelectedWorkFlow.Copy(CurrentWorkFlow);
            //}
        }

        public void Deactivate()
        {
            var isDeactive = Common.License.Request("deactivation");
            MessageBox.Show("The license has been deactivated . Goodbye!");
        }


        public void Clear()
        {
            CurrentWorkFlow = new WorkFlowViewModel();
            SelectedWorkFlow = null;
            SelectedWorkFlow = new WorkFlowViewModel();
            //_selectedWorkFlow = null;
            //OnPropertyChanged("SelectedWorkFlow");
        }

        //public void ClearMessage()
        //{
        //    LogMessagePs = string.Empty;
        //    VisibilityLog = "Collapsed";
        //}

        public async Task Delete()
        {
            await Task.Run(() =>
            {
                ////This used to live in the LoadWorkflows method on its own
                IsLoading = true;
                StatusBar = "Deleting workflow, please wait...";

            });

            StatusBar = "";
            IsLoading = false;
            ////

        }

        ICommand onButtonClickCommand;

        /// <summary>
        /// Button click RelayCommand
        /// //http://www.codeproject.com/Tips/665546/Passing-Command-Parameter-for-Buttons-within-an
        //SEARCH: MVVM XAML button method with parameters
        /// </summary>
        public ICommand OnButtonClickCommand
        {
            get
            {
                return onButtonClickCommand ??
                    (onButtonClickCommand = new RelayCommand(ButtonClick));
            }
        }

        /// <summary>
        /// Handles button clicks according to the CommandParameter passed from XAML
        /// </summary>
        /// <param name="button"></param>
        private void ButtonClick(object button)
        {
            //MessageBox.Show(button.ToString());

            if (button.ToString() == "AudioWelcomeSelector")
            {
                var audioFilePath = FileHelper.AudioFileSelector();

                if (audioFilePath != null)
                {
                    var audioFileName = audioFilePath.Substring(audioFilePath.LastIndexOf(@"\") + 1);

                    CurrentWorkFlow.AudioWelcomeFilePath = audioFilePath;
                    CurrentWorkFlow.AudioWelcome = audioFileName;
                }
            }

            if (button.ToString() == "AudioWelcomeRemove")
            {
                CurrentWorkFlow.AudioWelcomeFilePath = null;
                CurrentWorkFlow.AudioWelcome = null;
            }

            if (button.ToString() == "AudioAfterHoursSelector")
            {
                var audioFilePath = FileHelper.AudioFileSelector();

                if (audioFilePath != null)
                {
                    var audioFileName = audioFilePath.Substring(audioFilePath.LastIndexOf(@"\") + 1);

                    CurrentWorkFlow.AudioAfterHoursFilePath = audioFilePath;
                    CurrentWorkFlow.AudioAfterHours = audioFileName;
                }
            }

            if (button.ToString() == "AudioAfterHoursRemove")
            {
                CurrentWorkFlow.AudioAfterHoursFilePath = null;
                CurrentWorkFlow.AudioAfterHours = null;
            }

            if (button.ToString() == "AudioHolidaysSelector")
            {
                var audioFilePath = FileHelper.AudioFileSelector();

                if (audioFilePath != null)
                {
                    var audioFileName = audioFilePath.Substring(audioFilePath.LastIndexOf(@"\") + 1);

                    CurrentWorkFlow.AudioHolidaysFilePath = audioFilePath;
                    CurrentWorkFlow.AudioHolidays = audioFileName;
                }
            }

            if (button.ToString() == "AudioHolidaysRemove")
            {
                CurrentWorkFlow.AudioHolidaysFilePath = null;
                CurrentWorkFlow.AudioHolidays = null;
            }

            if (button.ToString() == "AudioIvrSelector")
            {
                var audioFilePath = FileHelper.AudioFileSelector();

                if (audioFilePath != null)
                {
                    var audioFileName = audioFilePath.Substring(audioFilePath.LastIndexOf(@"\") + 1);

                    CurrentWorkFlow.AudioIvrFilePath = audioFilePath;
                    CurrentWorkFlow.AudioIvr = audioFileName;
                }
            }

            if (button.ToString() == "AudioIvrRemove")
            {
                CurrentWorkFlow.AudioIvrFilePath = null;
                CurrentWorkFlow.AudioIvr = null;
            }

            if (button.ToString() == "AudioHoldMusicSelector")
            {
                var audioFilePath = FileHelper.AudioFileSelector();

                if (audioFilePath != null)
                {
                    var audioFileName = audioFilePath.Substring(audioFilePath.LastIndexOf(@"\") + 1);

                    CurrentWorkFlow.AudioHoldMusicFilePath = audioFilePath;
                    CurrentWorkFlow.AudioHoldMusic = audioFileName;
                }

            }

            if (button.ToString() == "AudioHoldMusicRemove")
            {
                CurrentWorkFlow.AudioHoldMusicFilePath = null;
                CurrentWorkFlow.AudioHoldMusic = null;
            }
        }

        public WorkFlowViewModel SelectedWorkFlow
        {
            get { return _selectedWorkFlow; }
            set
            {
                if (_selectedWorkFlow != value)// && value != null
                {
                    _selectedWorkFlow = value;

                    if (_selectedWorkFlow != null)
                    {
                        _selectedWorkFlow.DesignerViewModel = this;
                        CurrentWorkFlow = (WorkFlowViewModel)value.Clone();
                    }
                    OnPropertyChanged("SelectedWorkFlow");
                }
            }
        }

        public WorkFlowViewModel CurrentWorkFlow
        {
            get { return _currentWorkFlow; }
            set
            {
                //if (_currentWorkFlow != value && value != null)
                if (_currentWorkFlow != value)
                {
                    _currentWorkFlow = value;
                    _currentWorkFlow.DesignerViewModel = this;
                    OnPropertyChanged("CurrentWorkFlow");
                }
            }
        }


        /// <summary>
        /// Auto-complete: Tracks the state of the search results pop-up
        /// </summary>
        public bool PopupOpen
        {
            get
            {
                return _popupOpen;
            }

            set
            {
                if (_popupOpen != value)
                {
                    _popupOpen = value;
                    OnPropertyChanged("PopupOpen");
                }
            }
        }

        /// <summary>
        /// Auto-complete: Holds the search query
        /// </summary>
        public string SearchQuery
        {
            get { return _searchQuery; }
            set
            {
                if (_searchQuery != value)
                {
                    _searchQuery = value;

                    OnChangeSearchQuery();
                    OnPropertyChanged("SearchQuery");
                }
            }
        }

        /// <summary>
        /// Auto-complete: Holds the value of the selected search result
        /// </summary>
        public object AutoCompleteSelection
        {
            get { return _autoCompleteSelection; }
            set
            {
                if (_autoCompleteSelection != value)
                {
                    _autoCompleteSelection = value;
                    OnPropertyChanged("AutoCompleteSelection");

                    PopupOpen = false;

                    //Holds selected item
                    var number = _autoCompleteSelection as Number;
                    var user = _autoCompleteSelection as UserViewModel;
                    var queue = _autoCompleteSelection as QueueViewModel;
                    var wf = _autoCompleteSelection as WorkFlowViewModel;
                    //var test = _autoCompleteSelection as TestItem;

                    if (number != null)
                    {
                        //_searchQuery = String.Concat(number.AssignedTo, " \t DDI:  \t", number.DDI);
                        _searchQuery = String.Concat(number.DDI);
                    }
                    //else
                    //{
                    if (queue != null)
                    {
                        //_searchQuery = String.Concat(user.DisplayName, "\t Sip Address: \t", user.SipAddress);
                        _searchQuery = String.Concat(queue.Name);
                    }
                    if (wf != null)
                    {
                        //_searchQuery = String.Concat(user.DisplayName, "\t Sip Address: \t", user.SipAddress);
                        _searchQuery = String.Concat(wf.Uri + "@" + wf.SipDomain);
                    }
                    if (user != null)
                    {
                        //_searchQuery = String.Concat(user.DisplayName, "\t Sip Address: \t", user.SipAddress);
                        _searchQuery = String.Concat(user.SipAddress);
                    }
                    //if (test != null)
                    //{
                    //    _searchQuery = String.Concat(test.TestName, "\t Description: \t", test.TestDescription);
                    //}
                    //}

                    OnPropertyChanged("SearchQuery");
                }
            }
        }

        /// <summary>
        /// Auto-complete: Handles a change in auto-complete search query
        /// </summary>
        private void OnChangeSearchQuery()
        {
            //If search query is empty return
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                PopupOpen = false;
                return;
            }

            //Search will be performed in upper case
            var searchUpperQuery = SearchQuery.ToUpper();

            //Search Source - Numbers
            NumbersFiltered.Clear();
            //foreach (var number in Numbers.Where(number => number.AssignedTo.ToUpper().Contains(searchUpperQuery) ||
            //    number.DDI.ToUpper().Contains(searchUpperQuery) ||
            //    number.Ext.ToUpper().Contains(searchUpperQuery)))
            //{
            //    NumbersFiltered.Add(number);
            //}

            var Users = _dataService.GetUsers(false); //THIS IS VERY INEFFICIENT I THINK - Prob delegate to return value once loaded by users tab?
            //Search Source - Users
            UsersFiltered.Clear();
            foreach (var item in Users.Where(item => item.DisplayName.ToUpper().Contains(searchUpperQuery) ||
                item.SipAddress.ToUpper().Contains(searchUpperQuery)))
            {
                UsersFiltered.Add(item);
            }

            var Queues = _dataService.GetQueues(false); //THIS IS VERY INEFFICIENT I THINK - Prob delegate to return value once loaded by users tab?
            //Search Source - Users
            QueuesFiltered.Clear();
            foreach (var item in Queues.Where(item => item.Name.ToUpper().Contains(searchUpperQuery) ||
                item.Description.ToUpper().Contains(searchUpperQuery)))
            {
                QueuesFiltered.Add(item);
            }

            var Wf = _dataService.GetWorkflows(); //THIS IS VERY INEFFICIENT I THINK - Prob delegate to return value once loaded by users tab?
            //Search Source - Users
            WfFiltered.Clear();
            foreach (var item in Wf.Where(item => item.Name.ToUpper().Contains(searchUpperQuery) ||
                item.Uri.ToUpper().Contains(searchUpperQuery) || item.SipDomain.ToUpper().Contains(searchUpperQuery)))
            {
                WfFiltered.Add(item);
            }


            //Search Source - TestCollection
            //TestsFiltered.Clear();
            //foreach (var test in TestCollection.Where(test => test.TestId.ToUpper().Contains(searchUpperQuery) ||
            //    test.TestDescription.ToUpper().Contains(searchUpperQuery) ||
            //    test.TestName.ToUpper().Contains(searchUpperQuery)))
            //{
            //    TestsFiltered.Add(test);
            //}

            if (!PopupOpen)
            {
                PopupOpen = true;
            }
        }

        /// <summary>
        /// Auto-complete: Clears the current search query
        /// </summary>
        private void RemoveSearch()
        {
            _searchQuery = String.Empty;
            OnChangeSearchQuery();
            OnPropertyChanged("SearchQuery");
        }


        //Class wide variables
        private WorkFlowViewModel _selectedWorkFlow;
        private WorkFlowViewModel _currentWorkFlow;

        private IList<WorkFlowViewModel> _loadedWorkflows;

        private readonly IDataService _dataService = DataServiceFactory.Get();

        //private readonly BackgroundWorker _backgroundWorker;
        private bool _isLoading;
        private bool _isEnabled;
        private string _statusBar;

        private string _visibilityLog;
        private string _logMessagePs;
        private IEnumerable<BusinessHourGroupViewModel> _businessHoursGroupsFiltered;

        public ICommand RemoveRootIvrCommand { get; set; }

        public ObservableCollection<QueueViewModel> Queues { get; set; }

        public ObservableCollection<BusinessHourGroupViewModel> BusinessHoursGroups { get; private set; }

        public ObservableCollection<HolidayGroupViewModel> HolidaysGroups { get; private set; }

        //        public ICommand ClearMessageCommand { get; private set; }
        public ICommand LoadCommand { get; private set; }
        public ICommand CreateCommand { get; private set; }
        public ICommand ClearCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand DeactivateCommand { get; private set; }
        public ICommand AudioWelcomeSelectorCommand { get; private set; }
        public ICommand AudioAfterHoursSelectorCommand { get; private set; }
        public ICommand AudioHolidaysSelectorCommand { get; private set; }
        public ICommand AudioIvrSelectorCommand { get; private set; }
        public ICommand AudioHoldMusicSelectorCommand { get; private set; }

        public Dictionary<string, string> Destinations { get; private set; }

        public Dictionary<string, string> Languages { get; private set; }

        public Dictionary<string, string> TimeZones { get; private set; }

        public List<string> SipDomains { get; set; }
        public ObservableCollection<WorkFlowViewModel> WorkFlows { get; private set; }
        //private List<WorkFlowViewModel> _workFlowsLoaded { get; set; }
        public IEnumerable<string> Pools { get; private set; }


        //Auto-compete
        private bool _popupOpen;
        private string _searchQuery;
        private object _autoCompleteSelection;
        public ICommand RemoveSearchCommand { get; private set; }
        public ObservableCollection<Number> NumbersFiltered { get; set; }
        public ObservableCollection<UserViewModel> UsersFiltered { get; set; }
        public ObservableCollection<QueueViewModel> QueuesFiltered { get; set; }
        public ObservableCollection<WorkFlowViewModel> WfFiltered { get; set; }
        //public ObservableCollection<TestItem> TestsFiltered { get; set; }

        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;

                return error;
            }
        }
        public string Error
        {
            get { throw new NotImplementedException(); }
        }

    }
}


//(CurrentWorkFlow.AfterHoursDestination, CurrentWorkFlow.AfterHoursGroup, CurrentWorkFlow.AfterHoursMessage, CurrentWorkFlow.AfterHoursSipDomain, CurrentWorkFlow.AfterHoursUri,
//    CurrentWorkFlow.AudioAfterHours, CurrentWorkFlow.AudioAfterHoursFilePath, CurrentWorkFlow.AudioHoldMusic, CurrentWorkFlow.AudioHoldMusicFilePath, CurrentWorkFlow.AudioHolidays, CurrentWorkFlow.AudioHolidaysFilePath,
//    CurrentWorkFlow.AudioIvr, CurrentWorkFlow.AudioIvrFilePath, CurrentWorkFlow.AudioWelcome, CurrentWorkFlow.AudioWelcomeFilePath, CurrentWorkFlow.BusinessHourDestination, CurrentWorkFlow.BusinessHoursGroup,
//    CurrentWorkFlow.Description, CurrentWorkFlow.DisplayNumber, CurrentWorkFlow.EnableAgentAnonymity, CurrentWorkFlow.EnableForFederation, CurrentWorkFlow.EnableIVRMode, CurrentWorkFlow.HolidayDestination,
//    CurrentWorkFlow.HolidayGroup, CurrentWorkFlow.HolidayMessage, CurrentWorkFlow.HolidaySipDomain, CurrentWorkFlow.HolidayUri, CurrentWorkFlow.IvrMessage, CurrentWorkFlow.IvrOptions, CurrentWorkFlow.Language,
//    CurrentWorkFlow.Name, CurrentWorkFlow.Number, CurrentWorkFlow.OwnerPool, CurrentWorkFlow.Queue, CurrentWorkFlow.Queues, CurrentWorkFlow.SipAddress, CurrentWorkFlow.SipDomain, CurrentWorkFlow.TimeZone,
//    CurrentWorkFlow.Uri, CurrentWorkFlow.WelcomeMessage)

//var set = new WorkflowsSetService();
//var wfObj = set.PrepareSetCsRgsWorkflow(
////WORKFLOW GENERAL PROPERTIES
//CurrentWorkFlow.OwnerPool,
//CurrentWorkFlow.Name,
//CurrentWorkFlow.Description,
//CurrentWorkFlow.Number,
//CurrentWorkFlow.DisplayNumber,
//CurrentWorkFlow.SipAddress,
//CurrentWorkFlow.Uri,
//CurrentWorkFlow.SipDomain,
//CurrentWorkFlow.EnableWorkflow, //this needs to be added to form etc
//CurrentWorkFlow.EnableAgentAnonymity, 
//CurrentWorkFlow.EnableForFederation,
//CurrentWorkFlow.Language,
//CurrentWorkFlow.TimeZone,
//CurrentWorkFlow.AudioHoldMusic, //prob dont need this, full file path OK
//CurrentWorkFlow.AudioHoldMusicFilePath,
////AFTER HOURS
//CurrentWorkFlow.BusinessHoursGroup, //not sure if this is required
//CurrentWorkFlow.AfterHoursGroup, //Name of after hours group
//CurrentWorkFlow.BusinessHourDestination, //not sure about this one
//CurrentWorkFlow.AfterHoursDestination,
//CurrentWorkFlow.AfterHoursMessage,
//CurrentWorkFlow.AfterHoursUri,
//CurrentWorkFlow.AfterHoursSipDomain,
//CurrentWorkFlow.AudioAfterHours, //prob dont need this, full file path OK
//CurrentWorkFlow.AudioAfterHoursFilePath,
////HOLIDAYS
//CurrentWorkFlow.HolidayGroup, //Name of holidays group
//CurrentWorkFlow.HolidayDestination,
//CurrentWorkFlow.HolidayMessage,
//CurrentWorkFlow.HolidayUri,
//CurrentWorkFlow.HolidaySipDomain,
//CurrentWorkFlow.AudioHolidays, //prob dont need this, full file path OK
//CurrentWorkFlow.AudioHolidaysFilePath,

////ACTIONS - DEFAULT ACTION (Equates to Welcome Message on the UI)
//CurrentWorkFlow.AudioWelcome, //prob dont need this, full file path OK
//CurrentWorkFlow.AudioWelcomeFilePath,
//CurrentWorkFlow.WelcomeMessage,

////Determine default action type - Queue or Question
//CurrentWorkFlow.Queue, //If queue selected, TransferToQueue - Not sure which is correct queue or queues
//CurrentWorkFlow.Queues,

//CurrentWorkFlow.EnableIVRMode,
//CurrentWorkFlow.AudioIvr, 
//CurrentWorkFlow.AudioIvrFilePath,
//CurrentWorkFlow.IvrMessage,
//CurrentWorkFlow.IvrOptions);

