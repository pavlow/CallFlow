using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using CallFlowManager.UI.Common;
using CallFlowManager.UI.Views;
using Logging;
using NLog;

namespace CallFlowManager.UI.ViewModels.Logs
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows.Input;

    public class LogsViewModel : PropertyChangedBase
    {
        private static readonly NLogger Logger = LoggerFactory.Instance.GetCurrentClassLogger() as NLogger;

        private const string LogMemoryFilePath = "LogCallFlow.log";

        private string _logFilePath;

//        private string _log;

        private FilterLogViewModel _filterContext = new FilterLogViewModel();

        private ObservableCollection<InfoItem> _entriesMemory = new ObservableCollection<InfoItem>();

 //       private ObservableCollection<InfoItem> _entriesFile = new ObservableCollection<InfoItem>();

        private List<string> _interestedLevels = new List<string>();

        private IEnumerable<InfoItem> _filteredList;
        
        public LogsViewModel()
        {
            LoadLogCommand = new RelayCommand(_ => LoadLog());
            CurrentLogCommand = new RelayCommand(_ => CurrentLog());
            ClearLogCommand = new RelayCommand(_ => ClearLog());
            FilterCommand = new RelayCommand(_ => SetFilter());

 //           Entries = new ObservableCollection<InfoItem>();
            Entries = _entriesMemory;
            SetLevels();
        }

        public ICommand LoadLogCommand { get; private set; }

        public ICommand ClearLogCommand { get; private set; }

        public ICommand FilterCommand { get; private set; }

        public ICommand CurrentLogCommand { get; private set; }


        //public ObservableCollection<LogEventInfo> LogEntries
        //{
        //    get { return Logger.LogEntries; }
        //}

        public ObservableCollection<InfoItem> Entries { get; set; }

        public IEnumerable<InfoItem> FilteredList
        {
            get
            {
                if (_filterContext.Filter.IsPs)
                {
                    _filteredList = from entry in Entries
                                    where (_interestedLevels.Contains(entry.Level) || entry.Logger.Contains("Ps"))
                                    select entry;
                }
                else
                {
                    _filteredList = from entry in Entries
                                    where _interestedLevels.Contains(entry.Level)
                                    select entry;
                }
                return _filteredList;
            }
            set
            {
                _filteredList = value;
                OnPropertyChanged("FilteredList");
            }
        }

        public void LoadLog(string fName = null)
        {
            _logFilePath = fName ?? FileHelper.LogFileSelector();

            if (_logFilePath != null)
            {
                Logger.Info("Loaded log file: \n" + _logFilePath);

                Entries = new ObservableCollection<InfoItem>();
                var text = File.ReadAllText(_logFilePath);
                string[] lines = text.Split(new[] { "|TR|" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in lines)
                {
                    string[] values = line.Trim('\n', '\r').Split(new[] { "|TD|" }, StringSplitOptions.None);
                    if (values.Count() > 3)
                    {
                        var message = values[1].Replace("\n\r\n\r\n\r", "\n\r");
                        message = message.Replace("\n\r\n\r", "\n\r");
                        this.Entries.Add(new InfoItem(values[0], message, values[2], values[3]));
                    }
                }
            }
            OnPropertyChanged("Entries");
        }

        public void ClearLog()
        {
            Entries = _entriesMemory;
            bool isValid = false;
            
                string sourceDir = Path.GetDirectoryName(LogMemoryFilePath);

                string dateTime = DateTime.Now.ToString("yyyy-MM-dd hh_mm_ss");
                string fileName = Path.GetFileNameWithoutExtension(LogMemoryFilePath);
                string archiveFileName = string.Concat(fileName, " ", dateTime, ".clg");
                string archiveLogFilePath = Path.Combine(sourceDir, "logs", "archive", archiveFileName);

                try
                {
                    using (var stream = File.CreateText(archiveLogFilePath))
                    {
                        var line = string.Empty;


                        foreach (var entry in Entries)
                        {
                            line = string.Concat(entry.DateItem, "|TD|", entry.Message, "|TD|", entry.Logger, "|TD|", entry.Level, "|TR|");
                            stream.WriteLine(line);
                        }
                        Logger.Info("Cleared logs in memory. File with archive is {0}", archiveFileName);
                    }
                }
                catch (Exception ex)
                {
                    var message = string.Concat("Error cleaning and archiving log file ", LogMemoryFilePath, "\n", ex.Message);
                    Logger.Info(message);
                    MessageBox.Show(message);
                    OnPropertyChanged("Entries");
                }
                _entriesMemory.Clear();
                
                OnPropertyChanged("Entries");
        }

        public void AddEntry(string messageLine)
        {
            {
                var line = messageLine;
                string[] values = line.Trim('\n', '\r').Split(new[] { "||" }, StringSplitOptions.None);
                if (values.Count() > 3)
                {
                    var message = values[1].Replace("\n\r\n\r\n\r", "\n\r");
                    message = message.Replace("\n\r\n\r", "\n\r");
                    _entriesMemory.Add(new InfoItem(values[0], message, values[3], values[2]));
                }
                OnPropertyChanged("FilteredList");
            }
        }

        private void SetFilter()
        {
            DialogFilter();
            SetLevels();
        }

        private void CurrentLog()
        {
            Entries = _entriesMemory;
            OnPropertyChanged("FilteredList");
        }

        private void DialogFilter()
        {
            var dialogResult = false;  // Нажата Ok в диалоговом окне

            var window = new FilterDialog
            {
                DataContext = _filterContext
            };

            _filterContext.RequestCloseDialogEvent += (sender, args) => window.Close();
            _filterContext.DialogResultTrueEvent += (sender, args) => dialogResult = true;

            window.ShowDialog();
        }

        private void SetLevels()
        {
            _interestedLevels.Clear();

            if (_filterContext.Filter.IsDebug)
            {
                _interestedLevels.Add("Debug");
            }
            if (_filterContext.Filter.IsError)
            {
                _interestedLevels.Add("Error");
            }
            if (_filterContext.Filter.IsFatal)
            {
                _interestedLevels.Add("Fatal");
            }
            if (_filterContext.Filter.IsInfo)
            {
                _interestedLevels.Add("Info");
            }
            if (_filterContext.Filter.IsTrace)
            {
                _interestedLevels.Add("Trace");
            }
            if (_filterContext.Filter.IsWarn)
            {
                _interestedLevels.Add("Warn");
            }

            OnPropertyChanged("FilteredList");
        }

    }
}