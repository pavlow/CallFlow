// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HolidaysViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The holidays view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using Lync_WCF;

namespace CallFlowManager.UI.ViewModels.Holidays
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;
    using Business;
    using Common;

    /// <summary>
    /// The holidays view model.
    /// </summary>
    public class HolidaysViewModel : PropertyChangedBase, IDataErrorInfo
    {
        private bool allPropertiesValid = false;

        // Class-wide Variables

        /// <summary>
        /// The _data service.
        /// </summary>
        private readonly IDataService _dataService = DataServiceFactory.Get();

        /// <summary>
        /// The _selected holiday group.
        /// </summary>
        private HolidayGroupViewModel _selectedHolidayGroup;

        /// <summary>
        /// The _new holiday group.
        /// </summary>
        private HolidayGroupViewModel _newHolidayGroup;

        /// <summary>
        /// The _selected holiday time.
        /// </summary>
        private HolidayTimeViewModel _selectedHolidayTime;

        /// <summary>
        /// The _new holiday time.
        /// </summary>
        private HolidayTimeViewModel _newHolidayTime;

        /// <summary>
        /// The _is loading.
        /// </summary>
        private bool _isLoading;

        /// <summary>
        /// The _is enabled.
        /// </summary>
        private bool _isEnabled;

        private bool _isUniqueHolidayGroupName;

        /// <summary>
        /// The _status bar.
        /// </summary>
        private string _statusBar;

        /// <summary>
        /// The _loaded holiday groups.
        /// </summary>
        private List<HolidayGroupViewModel> _loadedHolidayGroups;

        /// <summary>
        /// Initializes a new instance of the <see cref="HolidaysViewModel"/> class.
        /// </summary>
        public HolidaysViewModel()
        {
            // _backgroundWorker = new BackgroundWorker();
            // _backgroundWorker.DoWork += LoadHolidays;
            // _backgroundWorker.RunWorkerCompleted += ChangeStatus;
            _loadedHolidayGroups = new List<HolidayGroupViewModel>();
            HolidayGroups = new ObservableCollection<HolidayGroupViewModel>();

            Hours = Globals.TimeHours;

            // Hours = Enumerable.Range(0, 23);
            Minutes = Globals.TimeMinutes;

            // Pools = Enumerable.Range(0, 59);
            // Pools = _dataService.Pools;
            NewHolidayGroup = new HolidayGroupViewModel { Pools = Pools };
            NewHolidayTime = new HolidayTimeViewModel();

            LoadCommand = new RelayCommand(_ => { LoadHolidays(); });
            CreateCommand = new RelayCommand(_ => { Update(); });
            AddGroupCommand = new RelayCommand(_ => AddGroup());
            RemoveGroupCommand = new RelayCommand(_ => RemoveGroup());
            BulkAddGroupCommand = new RelayCommand(_ => BulkAddGroup());
            RemoveHolidayCommand = new RelayCommand(_ => RemoveHoliday());
            AddHolidayCommand = new RelayCommand(_ => AddHoliday());
            OpenHolidaysCsvCommand = new RelayCommand(_ => { OpenHolidaysCsv(); });
            SelectedHolidayGroup = _selectedHolidayGroup;
        }

        #region Start-End Date Time properties

        public DateTime StartDateTime
        {
            get { return NewHolidayTime.StartHolidayDate.DateTime; }
            set
            {
                if (NewHolidayTime.StartHolidayDate.DateTime != value)
                {
                    NewHolidayTime.StartHolidayDate.DateTime = value.Date.AddHours(StartHour).AddMinutes(StartMinute);
                    ForcePropertyChange();
                }
            }
        }

        public int StartHour
        {
            get { return NewHolidayTime.StartHolidayDate.Hour; }
            set
            {
                if (NewHolidayTime.StartHolidayDate.Hour != value)
                {
                    NewHolidayTime.StartHolidayDate.ChangeTime(value, NewHolidayTime.StartHolidayDate.Minute);
                    ForcePropertyChange();
                }
            }
        }

        public int StartMinute
        {
            get { return NewHolidayTime.StartHolidayDate.Minute; }
            set
            {
                if (NewHolidayTime.StartHolidayDate.Minute != value)
                {
                    NewHolidayTime.StartHolidayDate.ChangeTime(NewHolidayTime.StartHolidayDate.Hour, value);
                    ForcePropertyChange();
                }
            }
        }

        public DateTime EndDateTime
        {
            get { return NewHolidayTime.EndHolidayDate.DateTime; }
            set
            {
                if (NewHolidayTime.EndHolidayDate.DateTime != value)
                {
                    NewHolidayTime.EndHolidayDate.DateTime = value.Date.AddHours(EndHour).AddMinutes(EndMinute);
                    ForcePropertyChange();
                }
            }
        }

        public int EndHour
        {
            get { return NewHolidayTime.EndHolidayDate.Hour; }
            set
            {
                if (NewHolidayTime.EndHolidayDate.Hour != value)
                {
                    NewHolidayTime.EndHolidayDate.ChangeTime(value, NewHolidayTime.EndHolidayDate.Minute);
                    ForcePropertyChange();
                }
            }
        }

        public int EndMinute
        {
            get { return NewHolidayTime.EndHolidayDate.Minute; }
            set
            {
                if (NewHolidayTime.EndHolidayDate.Minute != value)
                {
                    NewHolidayTime.EndHolidayDate.ChangeTime(NewHolidayTime.EndHolidayDate.Hour, value);
                    ForcePropertyChange();
                }
            }
        }

        #endregion Start-End Date Time properties


        /// <summary>
        /// Gets the hours.
        /// </summary>
        public IEnumerable<int> Hours { get; private set; }

        /// <summary>
        /// Gets the minutes.
        /// </summary>
        public IEnumerable<int> Minutes { get; private set; }

        /// <summary>
        /// Gets the pools.
        /// </summary>
        public List<string> Pools { get; private set; }

        #region Commands

        /// <summary>
        /// Gets the load command.
        /// </summary>
        public ICommand LoadCommand { get; private set; }

        /// <summary>
        /// Gets the create command.
        /// </summary>
        public ICommand CreateCommand { get; private set; }

        /// <summary>
        /// Gets the add group command.
        /// </summary>
        public ICommand AddGroupCommand { get; private set; }

        /// <summary>
        /// Gets the remove group command.
        /// </summary>
        public ICommand RemoveGroupCommand { get; private set; }

        /// <summary>
        /// Gets the bulk add group command.
        /// </summary>
        public ICommand BulkAddGroupCommand { get; private set; }

        /// <summary>
        /// Gets the remove holiday command.
        /// </summary>
        public ICommand RemoveHolidayCommand { get; private set; }

        /// <summary>
        /// Gets the add holiday command.
        /// </summary>
        public ICommand AddHolidayCommand { get; private set; }

        /// <summary>
        /// Gets the OpenHolidaysCsvCommand
        /// </summary>
        public ICommand OpenHolidaysCsvCommand { get; private set; }

        #endregion Commands

        /// <summary>
        /// Gets the holiday groups.
        /// </summary>
        public ObservableCollection<HolidayGroupViewModel> HolidayGroups { get; private set; }

        /// <summary>
        /// Value to display on the status bar
        /// </summary>
        public string StatusBar
        {
            get
            {
                return _statusBar;
            }

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
            get
            {
                return _isLoading;
            }

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
            get
            {
                return _isEnabled;
            }

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
        /// Enable button Add Holiday Group Name
        /// </summary>
        public bool IsUniqueHolidayGroupName
        {
            get
            {
                return _isUniqueHolidayGroupName;
            }

            set
            {
                if (_isUniqueHolidayGroupName != value)
                {
                    _isUniqueHolidayGroupName = value;
                    OnPropertyChanged("IsUniqueHolidayGroupName ");
                }
            }
        }

        public bool IsUniqueHolidayName
        {
            get
            {
                return _isUniqueHolidayGroupName;
            }

            set
            {
                if (_isUniqueHolidayGroupName != value)
                {
                    _isUniqueHolidayGroupName = value;
                    OnPropertyChanged("IsUniqueHolidayName ");
                }
            }
        }

        public string HolidayGroupName
        {
            get { return NewHolidayGroup.Name; }
            set
            {
                if (NewHolidayGroup.Name != value)
                {
                    NewHolidayGroup.Name = value;

                    IsUniqueHolidayGroupName = !HolidayGroups.Any(p => (p.Name.Equals(value.Trim())) && p.OwnerPool.Equals(NewHolidayGroup.OwnerPool.Trim()));

                    OnPropertyChanged("HolidayGroupName");
                    OnPropertyChanged("IsUniqueHolidayGroupName");
                }
            }
        }

        public string HolidayName
        {
            get { return NewHolidayTime.Name; }
            set
            {
                if (NewHolidayTime.Name != value)
                {
                    NewHolidayTime.Name = value;

                    IsUniqueHolidayName = !SelectedHolidayGroup.Holidays.Any(p => (p.Name.Equals(value.Trim())));
                    OnPropertyChanged("HolidayName");
                    OnPropertyChanged("IsUniqueHolidayName");
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected holiday group.
        /// </summary>
        public HolidayGroupViewModel SelectedHolidayGroup
        {
            get
            {
                return _selectedHolidayGroup;
            }

            set
            {
                if (_selectedHolidayGroup != value)
                {
                    _selectedHolidayGroup = value;
                    if (value != null)
                    {
                        NewHolidayGroup = new HolidayGroupViewModel()
                        {
                            Name = _selectedHolidayGroup.Name,
                            OwnerPool = _selectedHolidayGroup.OwnerPool,
                            Pools = Pools
                        };
                    }
                    OnPropertyChanged("SelectedHolidayGroup");
                    OnPropertyChanged("HolidayGroupName");
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected holiday time.
        /// </summary>
        public HolidayTimeViewModel SelectedHolidayTime
        {
            get
            {
                return _selectedHolidayTime;
            }

            set
            {
                if (_selectedHolidayTime != value)
                {
                    _selectedHolidayTime = value;
                    if (value != null)
                    {
                        NewHolidayTime = (HolidayTimeViewModel)value.Clone();
                    }

                    OnPropertyChanged("SelectedHolidayTime");
                }
            }
        }

        /// <summary>
        /// Gets or sets the new holiday time.
        /// </summary>
        public HolidayTimeViewModel NewHolidayTime
        {
            get
            {
                return _newHolidayTime;
            }

            set
            {
                if (_newHolidayTime != value && value != null)
                {
                    _newHolidayTime = value;
                    OnPropertyChanged("NewHolidayTime");
                    OnPropertyChanged("IsUniqueHolidayName");
                }
            }
        }

        /// <summary>
        /// Gets or sets the new holiday group.
        /// </summary>
        public HolidayGroupViewModel NewHolidayGroup
        {
            get
            {
                return _newHolidayGroup;
            }

            set
            {
                if (_newHolidayGroup != value && value != null)
                {
                    _newHolidayGroup = value;
                    OnPropertyChanged("NewHolidayGroup");
                }
            }
        }

        #region Methods

        /// <summary>
        /// Runs the load task when the user selects the "Refresh" button. The load task will query SfB data.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task LoadHolidays()
        {
            var prevHolidayGroup = string.Empty;
            if (SelectedHolidayGroup != null)
            {
                prevHolidayGroup = SelectedHolidayGroup.Name;
            }

            await Task.Run(
                () =>
                {
                    ////This used to live in the LoadHolidays method on its own
                    IsLoading = true;
                    StatusBar = "Loading holidays, please wait...";

                    _loadedHolidayGroups.Clear();
                    //_dataService.LoadHolidays();
                    _dataService.GetHolidays(true, true);

                    // Shared
                    //_dataService.LoadRegistrarPools(); // Move to dependency of LoadHolidays //Done inPSDataService

                    // Move to a property::
                    Pools = _dataService.Pools;
                    OnPropertyChanged("Pools");

                    if (_dataService.HolidayGroups == null)
                    {
                        return;
                    }

                    foreach (var @group in _dataService.HolidayGroups.OrderBy(a => a.Name))
                    {
                        _loadedHolidayGroups.Add(new HolidayGroupViewModel(@group) { Pools = Pools });
                    }

                    ////
                });

            ////This used to live in the ChangeStatus method which was executed after the background worker completed
            ////Cannot run this within above thread!
            HolidayGroups.Clear();
            foreach (var holidayGroup in _loadedHolidayGroups)
            {
                HolidayGroupViewModel flow = holidayGroup;

                if (flow.Name.Equals(prevHolidayGroup))
                {
                    SelectedHolidayGroup = flow;
                }
                Application.Current.Dispatcher.Invoke((Action)(() => HolidayGroups.Add(flow)));
            }

            if (SelectedHolidayGroup == null)
            {
                SelectedHolidayGroup = HolidayGroups.FirstOrDefault();
            }

            OnPropertyChanged("SelectedHolidayGroup");
            StatusBar = string.Empty;
            IsLoading = false;

            ////
        }

        /// <summary>
        /// Runs the update task when the user selects the "Create/Update" button. The update task will commit changes to SfB.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task Update()
        {
            if (SelectedHolidayGroup == null)
            {
                StatusBar = "Select a holiday group to update and try again";
            }

            var prevHolidayGroup = string.Empty;

            if (SelectedHolidayGroup != null)
            {
                prevHolidayGroup = SelectedHolidayGroup.Name;
                IsLoading = true;
                StatusBar = "Updating holidays, please wait...";
                await Task.Run(
                    () =>
                    {
                        var set = new HolidaySetService();
                        var holObj = set.PrepareSetCsRgsHoliday(
                            SelectedHolidayGroup.Name,
                            SelectedHolidayGroup.OwnerPool,
                            SelectedHolidayGroup.Holidays);

                        var psQueries = new PsQueries();
                        psQueries.SetCsRgsHoliday(holObj);
                    });
                StatusBar = string.Empty;
                IsLoading = false;

                LoadHolidays();

                foreach (var holidayGroup in _loadedHolidayGroups)
                {
                    if (holidayGroup.Name.Equals(prevHolidayGroup))
                    {
                        SelectedHolidayGroup = holidayGroup;
                    }
                }
            }
        }

        /// <summary>
        /// Removes a holiday
        /// </summary>
        private void RemoveHoliday()
        {
            SelectedHolidayGroup.Holidays.Remove(SelectedHolidayTime);
            SelectedHolidayTime = null;
            OnPropertyChanged("SelectedHolidayTime");
            OnPropertyChanged("SelectedHolidayGroup");
        }

        /// <summary>
        /// Removes a holiday group
        /// </summary>
        private void RemoveGroup()
        {
            SelectedHolidayGroup.ResetHolidayTimes();
            HolidayGroups.Remove(SelectedHolidayGroup);
            SelectedHolidayGroup = null;
            OnPropertyChanged("SelectedHolidayGroup");
            OnPropertyChanged("HolidayGroupName");
        }

        /// <summary>
        /// Adds a holiday
        /// </summary>
        private void AddHoliday()
        {
            var holidayTime = (HolidayTimeViewModel)NewHolidayTime.Clone();
            if (holidayTime == null || SelectedHolidayGroup == null)
            {
                return;
            }

            if (SelectedHolidayGroup.Holidays.Any(p => p.Name.Contains(holidayTime.Name)))
            {
                MsgBox.Error(string.Format("Holiday {0} already exists", holidayTime.Name));
            }
            else
            {
                if (NewHolidayTime.EndHolidayDate.DateTime < NewHolidayTime.StartHolidayDate.DateTime)
                {
                    MsgBox.Error("End Date is before then Start Date");
                    NewHolidayTime = new HolidayTimeViewModel();
                    OnPropertyChanged("NewHolidayGroup");
                }
                else
                {
                    SelectedHolidayGroup.Holidays.Add(holidayTime);
                }
            }

            NewHolidayTime = new HolidayTimeViewModel();

            ForcePropertyChange();
            // NewHolidayTime = (HolidayTimeViewModel)NewHolidayTime.Clone();
        }

        private void ForcePropertyChange()
        {
            OnPropertyChanged("StartDateTime");
            OnPropertyChanged("StartHour");
            OnPropertyChanged("StartMinute");
            OnPropertyChanged("EndDateTime");
            OnPropertyChanged("EndHour");
            OnPropertyChanged("EndMinute");
        }

        private HolidayGroupViewModel AddGroupFromCsv(string pool, string name)
        {
            var holidayGroup = new HolidayGroupViewModel() { Name = name, OwnerPool = pool, Pools = Pools };
            if (!HolidayGroups.Any(p => (p.Name.Equals(name))))
            {
                // Add new HolidayGroup
                HolidayGroups.Add(holidayGroup);
            }
            else
            {
                // Return the found group if such already is
                var selectHolidayGroups = from selectGroup in HolidayGroups
                                          where (selectGroup.Name.Equals(name))
                                          select selectGroup;
                holidayGroup = selectHolidayGroups.FirstOrDefault();
                // Redefine pool from CSV (update)
                if (holidayGroup != null) holidayGroup.OwnerPool = pool;
            }
            return holidayGroup;
        }


        /// <summary>
        /// Adds a holiday group
        /// </summary>
        private void AddGroup()
        {
            if ((NewHolidayGroup.Name != null) && (NewHolidayGroup.OwnerPool != null))
            {
                if (HolidayGroups.Any(p => (p.Name.Equals(NewHolidayGroup.Name)) && p.OwnerPool.Equals(NewHolidayGroup.OwnerPool)))
                {
                    MsgBox.Error(string.Format("Holiday Group {0} already exists", NewHolidayGroup.Name));
                }
                else
                {
                    HolidayGroups.Add(new HolidayGroupViewModel() { Name = NewHolidayGroup.Name, OwnerPool = NewHolidayGroup.OwnerPool, Pools = Pools });
                }
            }
        }

        /// <summary>
        /// Open file dialogue for CSV file import, Adds holiday groups
        /// </summary>
        private void BulkAddGroup()
        {

            var csvFilePath = FileHelper.CsvFileSelector();

            if (csvFilePath != null)
            {
                var fileSteram = File.OpenRead(csvFilePath);
                var reader = new StreamReader(fileSteram);

                {
                    var firstLine = true;

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();

                        if (line == null) continue;
                        if (firstLine)
                        {
                            // skip header
                            firstLine = false;
                        }
                        else
                        {
                            string[] values = line.Split(',');

                            // HolidayGroupPool,HolidayGroup,HolidayName,HolidayStartDate(d.mm.yy),HolidayStartTime(hh:mm),HolidayEndDate,HolidayEndTime 

                            // Don't add Pool's that are in the CSV to the combobox.
                            //        if (!Pools.Contains(values[0]))
                            //        {
                            //            Pools.Add(values[0]);
                            //        }

                            if (values.Count() != 7)
                            {
                                continue;
                            }

                            // If we add a pool in the CSV that doesn't exist in the drop down
                            // that it should just add empty string to Pools (and show blank)
                            if (!Pools.Contains(values[0]))
                            {
                                if (!Pools.Contains(String.Empty))
                                {
                                    Pools.Add(String.Empty);
                                }
                                // Clear HolidayGroupPool
                                values[0] = String.Empty;
                            }

                            if ((values[0] != null) && (values[1] != null))
                            {
                                // Add item to HolidayGroups
                                var addHolidayGroup = AddGroupFromCsv(values[0], values[1]);

                                // Add item to Holidays in HolidayGroup
                                var holidayTime = new HolidayTimeViewModel(values[2], values[3], values[4], values[5],
                                    values[6]);
                                addHolidayGroup.AddHoliday(holidayTime, true);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Opens the example holidays Csv
        /// </summary>
        private void OpenHolidaysCsv()
        {
            //Process.Start(@"Assets\Holidays.csv");

            //string filePath = null;

            //configure save file dialog box
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.DefaultExt = "csv";
            saveFileDialog.Filter = "Csv files (*.csv)|*.csv |All files (*.*)|*.*";

            // Show save file dialog box
            Nullable<bool> result = saveFileDialog.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                //string filename = dlg.FileName;
                var filePath = saveFileDialog.FileName;
                var sourcePath = AppDomain.CurrentDomain.BaseDirectory + @"Assets\Holidays.csv";
                //File.WriteAllText(saveFileDialog.FileName, @"E:\Code\Andrew\Workspaces\Call Flow Manager\CallFlowManager\CallFlowManager.UI\bin\Debug\AssetsHolidays.csv");
                File.Copy(sourcePath, filePath, true);
                //File.WriteAllBytes(@filePath, Properties.Resources.importPurchases);
                //MessageBox.Show("File Successfully saved.\r\n\r\n" + filePath, "Success Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        #endregion Methods

        #region Validate

        public string this[string columnName]
        {
            get
            {
                string error = string.Empty;
                switch (columnName)
                {
                    case "StartDateTime":
                    case "EndDateTime":
                    case "StartHour":
                    case "StartMinute":
                    case "EndHour":
                    case "EndMinute":
                        if (NewHolidayTime.StartHolidayDate.DateTime > NewHolidayTime.EndHolidayDate.DateTime)
                            error = "Start date&time can't be later than End";
                        break;
                }

                //string error = (NewHolidayTime as IDataErrorInfo)[propertyName];
                //validProperties[propertyName] = String.IsNullOrEmpty(error) ? true : false;
                //ValidateProperties();
                //CommandManager.InvalidateRequerySuggested();

                return error;
            }
        }

        public string Error
        {
            // get { return (NewHolidayTime as IDataErrorInfo).Error; }
            get { throw new NotImplementedException(); }
        }

        #endregion Validate

        #region TO CLEAR

        //private void ValidateProperties()
        //{
        //    foreach (bool isValid in validProperties.Values)
        //    {
        //        if (!isValid)
        //        {
        //            this.AllPropertiesValid = false;
        //            return;
        //        }
        //    }
        //    this.AllPropertiesValid = true;
        //}


        //public bool AllPropertiesValid
        //{
        //    get { return allPropertiesValid; }
        //    set
        //    {
        //        if (allPropertiesValid != value)
        //        {
        //            allPropertiesValid = value;
        //            base.OnPropertyChanged("AllPropertiesValid");
        //        }
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the start date.
        ///// </summary>
        //public DateTime StartDate
        //{
        //    get
        //    {
        //        return _startDate;
        //    }

        //    set
        //    {
        //        if (_startDate != value)
        //        {
        //            _startDate = value;
        //            OnPropertyChanged("StartDate");
        //        }
        //    }
        //}

        ///// <summary>
        ///// Gets or sets the end date.
        ///// </summary>
        //public DateTime EndDate
        //{
        //    get
        //    {
        //        return _endDate;
        //    }

        //    set
        //    {
        //        if (_endDate != value)
        //        {
        //            _endDate = value;
        //            OnPropertyChanged("EndDate");
        //        }
        //    }
        //}

        ///// <summary>
        ///// The _start date.
        ///// </summary>
        //private DateTime _startDate;

        ///// <summary>
        ///// The _end date.
        ///// </summary>
        //private DateTime _endDate;

        // private Dictionary<string, bool> validProperties;
        #endregion TO CLEAR
    }
}