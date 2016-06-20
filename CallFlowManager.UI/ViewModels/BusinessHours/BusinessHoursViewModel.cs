using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CallFlowManager.UI.Business;
using CallFlowManager.UI.Common;

namespace CallFlowManager.UI.ViewModels.BusinessHours
{
    public class BusinessHoursViewModel : PropertyChangedBase, IDataErrorInfo
    {
        //Class-wide Variables
        private readonly IDataService _dataService = DataServiceFactory.Get();
        private BusinessHourGroupViewModel _selectedHourGroup;
        private BusinessHourGroupViewModel _newHourGroup;
        private OpenCloseTimeViewModel _openCloseTime;
        private OpenCloseTimeViewModel _newOpenCloseTime;
        private readonly BackgroundWorker _backgroundWorker;
        private bool _isLoading;
        private bool _isEnabled;
        private bool _isUniqueDayOfWeek;
        private string _messageTime1;
        private string _messageTime2;
        private string _statusBar;
        private List<BusinessHourGroupViewModel> _loadedBusinessHoursGroups;

        public BusinessHoursViewModel()
        {
            _loadedBusinessHoursGroups = new List<BusinessHourGroupViewModel>();
            Hours = Globals.TimeHours; //Enumerable.Range(0, 23);
            Minutes = Globals.TimeMinutes; //Enumerable.Range(0, 59);
            DayOfWeeks = Globals.DaysOfWeek;
            NewOpenCloseTime = new OpenCloseTimeViewModel();
            NewHourGroup = new BusinessHourGroupViewModel();
            BusinessHoursGroups = new ObservableCollection<BusinessHourGroupViewModel>();
            //Load = new RelayCommand(_ => { if (!_backgroundWorker.IsBusy)_backgroundWorker.RunWorkerAsync(); });

            UpdateCommand = new RelayCommand(_ => Update());
            LoadCommand = new RelayCommand(_ => { LoadBusinessHours(); });
            AddBusinessHoursCommand = new RelayCommand(_ => AddBusinessHours());
            AddGroupCommand = new RelayCommand(_ => AddGroup());
            RemoveGroupCommand = new RelayCommand(_ => RemoveGroup());
            RemoveBusinessHoursCommand = new RelayCommand(_ => RemoveBusinessHours());

            _newOpenCloseTime.PropertyDayOfWeekChanged += (sender, args) =>
            {
                IsUniqueDayOfWeek = CheckUniqueDayOfWeek();
            };
            //_newOpenCloseTime.OpenTime1.PropertyChanged += (sender, args) => CheckTimeError();
            //_newOpenCloseTime.OpenTime2.PropertyChanged += (sender, args) => CheckTimeError();
            //_newOpenCloseTime.CloseTime1.PropertyChanged += (sender, args) => CheckTimeError();
            //_newOpenCloseTime.CloseTime2.PropertyChanged += (sender, args) => CheckTimeError();
            _newOpenCloseTime.PropertyChanged += (sender, args) => ForcePropertyChange();
        }

        public ICommand UpdateCommand { get; private set; }
        public IEnumerable<int> Hours { get; private set; }
        public IEnumerable<int> Minutes { get; private set; }
        public IEnumerable<string> Pools { get; private set; }
        public Dictionary<string, string> DayOfWeeks { get; private set; }
        public ICommand LoadCommand { get; private set; }
        public ICommand AddBusinessHoursCommand { get; private set; }
        public ICommand AddGroupCommand { get; private set; }
        public ICommand RemoveGroupCommand { get; private set; }
        public ICommand RemoveBusinessHoursCommand { get; private set; }

        public ObservableCollection<BusinessHourGroupViewModel> BusinessHoursGroups { get; private set; }

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

        //public string MessageTime1
        //{
        //    get { return _messageTime1; }
        //    set
        //    {
        //        if (_messageTime1 != value)
        //        {
        //            _messageTime1 = value;
        //        }
        //        OnPropertyChanged("MessageTime1");
        //    }
        //}

        //public string MessageTime2
        //{
        //    get { return _messageTime2; }
        //    set
        //    {
        //        if (_messageTime2 != value)
        //        {
        //            _messageTime2 = value;
        //        }
        //        OnPropertyChanged("MessageTime2");
        //    }
        //}


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

        public bool IsUniqueDayOfWeek
        {
            get { return _isUniqueDayOfWeek; }
            set
            {
                if (_isUniqueDayOfWeek != value)
                {
                    _isUniqueDayOfWeek = value;
                    OnPropertyChanged("IsUniqueDayOfWeek");
                }
            }
        }
        
        public bool IsUniqueGroupName { get; private set; }

        public string HoursGroupName
        {
            get { return NewHourGroup.Name; }
            set
            {
                if (NewHourGroup.Name != value)
                {
                    NewHourGroup.Name = value;

                    IsUniqueGroupName = !BusinessHoursGroups.Any(p => (p.Name.Equals(value.Trim())) && p.OwnerPool.Equals(NewHourGroup.OwnerPool.Trim()));

                    OnPropertyChanged("HoursGroupName");
                    OnPropertyChanged("IsUniqueGroupName");
                }
            }
        }


        /// <summary>
        /// Runs the load task when the user selects the "Refresh" button. The load task will query SfB data.
        /// </summary>
        /// <returns></returns>
        public async Task LoadBusinessHours()
        {
            var prevHourGroup = string.Empty;
            if (SelectedHourGroup != null)
            {
                prevHourGroup = SelectedHourGroup.Name;
            }

            await Task.Run(() =>
            {
                ////This used to live in the LoadBusinessHours method on its own
                IsLoading = true;
                StatusBar = "Loading business hours, please wait...";
                _loadedBusinessHoursGroups.Clear();
                //_dataService.LoadBusinessHours();
                _dataService.GetBusinessHours(true, true);

                //Move to a property::
                Pools = _dataService.Pools;
                OnPropertyChanged("Pools");

                foreach (var @group in _dataService.BusinessHoursGroups.OrderBy(a => a.Name))
                {
                    _loadedBusinessHoursGroups.Add(new BusinessHourGroupViewModel(@group));
                }
                ////

            });
            ////This used to live in the ChangeStatus method which was executed after the background worker completed
            ////Cannot run this within above thread!
            BusinessHoursGroups.Clear();
            foreach (var businessHourGroup in _loadedBusinessHoursGroups)
            {
                BusinessHourGroupViewModel flow = businessHourGroup;
                if (flow.Name.Equals(prevHourGroup))
                {
                    SelectedHourGroup = flow;
                }

                Application.Current.Dispatcher.Invoke((Action)(() => BusinessHoursGroups.Add(flow)));
            }

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
            if (SelectedHourGroup == null)
            {
                StatusBar = "Select a business hour group to update and try again";
            }

            if (SelectedHourGroup != null)
            {
                var prevHourGroup = SelectedHourGroup.Name;
                IsLoading = true;
                StatusBar = "Updating business hours, please wait...";
                await Task.Run(() =>
                {
                    var set = new BusinessHoursSetService();
                    var busHoursObj = set.PrepareSetCsRgsHoursOfBusiness(SelectedHourGroup.Name,
                        SelectedHourGroup.OwnerPool, SelectedHourGroup.OpenCloseTimes);

                    var psQueries = new PsQueries();
                    psQueries.SetCsRgsHoursOfBusiness(busHoursObj);
                });
                StatusBar = "";
                IsLoading = false;
                LoadBusinessHours();

                foreach (var hourGroup in _loadedBusinessHoursGroups)
                {
                    if (hourGroup.Name.Equals(prevHourGroup))
                    {
                        SelectedHourGroup = hourGroup;
                    }
                }
            }
        }

        /// <summary>
        /// New Business Hours group
        /// </summary>
        public BusinessHourGroupViewModel NewHourGroup
        {
            get { return _newHourGroup; }
            set
            {
                if (_newHourGroup != value)
                {
                    _newHourGroup = value;
                    OnPropertyChanged("NewHourGroup");
                }
            }
        }

        /// <summary>
        /// Selected Busines Hours Group
        /// </summary>
        public BusinessHourGroupViewModel SelectedHourGroup
        {
            get { return _selectedHourGroup; }
            set
            {
                if (_selectedHourGroup != value)
                {
                    _selectedHourGroup = value;
                    IsUniqueDayOfWeek = CheckUniqueDayOfWeek();
                    if (value != null)
                    {
                        NewHourGroup = new BusinessHourGroupViewModel()
                        {
                            Name = _selectedHourGroup.Name,
                            OwnerPool = _selectedHourGroup.OwnerPool
                        };
                    }
                    OnPropertyChanged("SelectedHourGroup");
                }
            }
        }

        /// <summary>
        /// Selected Open/Close time
        /// </summary>
        public OpenCloseTimeViewModel SelectedOpenCloseTime
        {
            get { return _openCloseTime; }
            set
            {
                if (_openCloseTime != value)
                {
                    _openCloseTime = value;
                    if (value != null)
                    {
                        NewOpenCloseTime = (OpenCloseTimeViewModel)value.Clone();
                    }
                    OnPropertyChanged("SelectedOpenCloseTime");
                }
            }
        }

        /// <summary>
        /// New Open/Close time
        /// </summary>
        public OpenCloseTimeViewModel NewOpenCloseTime
        {
            get { return _newOpenCloseTime; }
            set
            {
                if (_newOpenCloseTime != value)
                {
                    _newOpenCloseTime = value;
                    OnPropertyChanged("NewOpenCloseTime");
                }
            }
        }

        /// <summary>
        /// Removes Business Hours Open/Close time
        /// </summary>
        private void RemoveBusinessHours()
        {
            SelectedHourGroup.OpenCloseTimes.Remove(SelectedOpenCloseTime);
            SelectedOpenCloseTime = null;
            OnPropertyChanged("SelectedOpenCloseTime");
            OnPropertyChanged("SelectedHourGroup");
        }

        /// <summary>
        /// Removes Business Hours Group
        /// </summary>
        private void RemoveGroup()
        {
            SelectedHourGroup.ResetOpenCloseTimes();
            BusinessHoursGroups.Remove(SelectedHourGroup);
            SelectedHourGroup = null;
            OnPropertyChanged("SelectedHourGroup");
            // TODO: implement removing from PS
        }

        /// <summary>
        /// Adds Business Hour Group
        /// </summary>
        private void AddGroup()
        {
            if (BusinessHoursGroups.Any(p => p.Name.Equals(NewHourGroup.Name)))
            {
                MsgBox.Error(string.Format("Business hour group {0} already exists", NewHourGroup.Name));
            }
            else
            {
                BusinessHoursGroups.Add(new BusinessHourGroupViewModel() { Name = NewHourGroup.Name, OwnerPool = NewHourGroup.OwnerPool });
            }
        }

        /// <summary>
        /// Adds Business Hours Open/Close time to group
        /// </summary>
        private void AddBusinessHours()
        {
            if (NewOpenCloseTime == null || SelectedHourGroup == null)
            {
                return;
            }

            var days = Globals.BusHoursDays(NewOpenCloseTime.DayOfWeek);
            foreach (var dayOfWeek in days)
            {
                if (!NewOpenCloseTime.OpenCloseTime1Enabled && !NewOpenCloseTime.OpenCloseTime2Enabled)
                {
                    continue;
                }

                OpenCloseTimeViewModel openCloseTime = NewOpenCloseTime.Clone() as OpenCloseTimeViewModel;
                if (openCloseTime != null)
                {
                    openCloseTime.DayOfWeek = dayOfWeek;

                    if (!openCloseTime.OpenCloseTime1Enabled)
                    {
                        openCloseTime.OpenTime1 = null;
                        openCloseTime.CloseTime1 = null;
                    }

                    if (!openCloseTime.OpenCloseTime2Enabled)
                    {
                        openCloseTime.OpenTime2 = null;
                        openCloseTime.CloseTime2 = null;
                    }

                    if (!OpenCloseTimeOverlaps(openCloseTime))
                    {
                        SelectedHourGroup.OpenCloseTimes.Add(openCloseTime);
                        //TODO: implement _dataService.Add();
                    }
                    else
                    {
                        MsgBox.Error(string.Format("Business hour for {0} already exists", openCloseTime.DayOfWeek));
                    }
                }
            }
        }

        /// <summary>
        /// Checks if the Open/Close time overlaps
        /// </summary>
        /// <param name="newOpenCloseTime"></param>
        /// <returns></returns>
        private bool OpenCloseTimeOverlaps(OpenCloseTimeViewModel newOpenCloseTime)
        {
            if (SelectedHourGroup.OpenCloseTimes.Any(p => p.DayOfWeek == newOpenCloseTime.DayOfWeek && (p.OpenTime1 != null && newOpenCloseTime.OpenTime1 != null || p.OpenTime2 != null && newOpenCloseTime.OpenTime2 != null)))
            {
                return true;
            }
            return false;
        }

        private bool CheckUniqueDayOfWeek()
        {
            var unique = false;
            if (SelectedHourGroup != null && SelectedHourGroup.OpenCloseTimes != null)
            {
                unique = !SelectedHourGroup.OpenCloseTimes.Any(p => p.DayOfWeek.Contains(NewOpenCloseTime.DayOfWeek));
                OnPropertyChanged("IsUniqueDayOfWeek");
                if (!unique)
                {
                    MessageBox.Show("Open/close time contains such day of week");
                }
            }
            return unique;
        }

        public int OpenTime1Hour
        {
            get { return NewOpenCloseTime.OpenTime1.Hour; }
            set
            {
                if (NewOpenCloseTime.OpenTime1.Hour != value)
                {
                    NewOpenCloseTime.OpenTime1.ChangeTime(value, NewOpenCloseTime.OpenTime1.Minute);
                    ForcePropertyChange();
                }
            }
        }

        public int OpenTime2Hour
        {
            get { return NewOpenCloseTime.OpenTime2.Hour; }
            set
            {
                if (NewOpenCloseTime.OpenTime2.Hour != value)
                {
                    NewOpenCloseTime.OpenTime2.ChangeTime(value, NewOpenCloseTime.OpenTime2.Minute);
                    ForcePropertyChange();
                }
            }
        }

        public int CloseTime1Hour
        {
            get { return NewOpenCloseTime.CloseTime1.Hour; }
            set
            {
                if (NewOpenCloseTime.CloseTime1.Hour != value)
                {
                    NewOpenCloseTime.CloseTime1.ChangeTime(value, NewOpenCloseTime.CloseTime1.Minute);
                    ForcePropertyChange();
                }
            }
        }

        public int CloseTime2Hour
        {
            get { return NewOpenCloseTime.CloseTime2.Hour; }
            set
            {
                if (NewOpenCloseTime.CloseTime2.Hour != value)
                {
                    NewOpenCloseTime.CloseTime2.ChangeTime(value, NewOpenCloseTime.CloseTime2.Minute);
                    ForcePropertyChange();
                }
            }
        }

        public int OpenTime1Minute
        {
            get { return NewOpenCloseTime.OpenTime1.Minute; }
            set
            {
                if (NewOpenCloseTime.OpenTime1.Minute != value)
                {
                    NewOpenCloseTime.OpenTime1.ChangeTime(NewOpenCloseTime.OpenTime1.Hour, value);
                    ForcePropertyChange();
                }
            }
        }

        public int OpenTime2Minute
        {
            get { return NewOpenCloseTime.OpenTime2.Minute; }
            set
            {
                if (NewOpenCloseTime.OpenTime2.Minute != value)
                {
                    NewOpenCloseTime.OpenTime2.ChangeTime(NewOpenCloseTime.OpenTime2.Hour, value);
                    ForcePropertyChange();
                }
            }
        }

        public int CloseTime1Minute
        {
            get { return NewOpenCloseTime.CloseTime1.Minute; }
            set
            {
                if (NewOpenCloseTime.CloseTime1.Minute != value)
                {
                    NewOpenCloseTime.CloseTime1.ChangeTime(NewOpenCloseTime.CloseTime1.Hour, value);
                    ForcePropertyChange();
                }
            }
        }

        public int CloseTime2Minute
        {
            get { return NewOpenCloseTime.CloseTime2.Minute; }
            set
            {
                if (NewOpenCloseTime.CloseTime2.Minute != value)
                {
                    NewOpenCloseTime.CloseTime2.ChangeTime(NewOpenCloseTime.CloseTime2.Hour, value);
                    ForcePropertyChange();
                }
            }
        }

        private void ForcePropertyChange()
        {
            OnPropertyChanged("OpenTime1Hour");
            OnPropertyChanged("OpenTime2Hour");
            OnPropertyChanged("CloseTime1Hour");
            OnPropertyChanged("CloseTime2Hour");
            OnPropertyChanged("OpenTime1Minute");
            OnPropertyChanged("OpenTime2Minute");
            OnPropertyChanged("CloseTime1Minute");
            OnPropertyChanged("CloseTime2Minute");

        }

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case "OpenTime1Hour":
                    case "OpenTime2Hour":
                    case "CloseTime1Hour":
                    case "CloseTime2Hour":
                    case "OpenTime1Minute":
                    case "OpenTime2Minute":
                    case "CloseTime1Minute":
                    case "CloseTime2Minute":
                        error = CheckTimeError();
                        break;
                }
                return error;
            }
        }

        public string Error { get; private set; }

        private string CheckTimeError()
        {
            string message = string.Empty;
            if ((NewOpenCloseTime.OpenTime1.DateTime.Hour * 60 + NewOpenCloseTime.OpenTime1.DateTime.Minute >
                NewOpenCloseTime.CloseTime1.DateTime.Hour * 60 + NewOpenCloseTime.CloseTime1.DateTime.Minute) &&
                NewOpenCloseTime.OpenCloseTime1Enabled)
            {
                message = "OpenTime 1 must be before CloseTime 1";
            }
            else
            {
                if ((NewOpenCloseTime.OpenTime2.DateTime.Hour * 60 + NewOpenCloseTime.OpenTime2.DateTime.Minute <
                NewOpenCloseTime.CloseTime1.DateTime.Hour * 60 + NewOpenCloseTime.CloseTime1.DateTime.Minute) &&
                NewOpenCloseTime.OpenCloseTime1Enabled &&
                NewOpenCloseTime.OpenCloseTime2Enabled)
                {
                    message = "OpenTime 2 must be after CloseTime 1";
                }
                else
                {
                    if ((NewOpenCloseTime.OpenTime2.DateTime.Hour * 60 + NewOpenCloseTime.OpenTime2.DateTime.Minute >
                        NewOpenCloseTime.CloseTime2.DateTime.Hour * 60 + NewOpenCloseTime.CloseTime2.DateTime.Minute) &&
                        NewOpenCloseTime.OpenCloseTime2Enabled)
                    {
                        message = "OpenTime 2 must be before CloseTime 2";
                    }
                }
            }
            return message;
        }
    }
}
