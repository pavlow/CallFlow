using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CallFlowManager.UI.Common;

namespace CallFlowManager.UI.ViewModels
{
    public sealed class MainWindowViewModel : PropertyChangedBase
    {

        static MainWindowViewModel _instance;

        public static MainWindowViewModel Instance

        {
            get { return _instance ?? (_instance = new MainWindowViewModel()); }

        }

        private MainWindowViewModel()
        {
            _visibilityLog = "Collapsed";
            ClearMessageCommand = new RelayCommand(_ => ClearMessage());
            _logMessagePs = "Ps messages:\n";
        }

        static MainWindowViewModel() { }

        private string _logMessagePs;
        private string _visibilityLog;

        public ICommand ClearMessageCommand { get; private set; }

        public string LogMessage
        {
            get { return _logMessagePs; }
            set
            {
                if (_logMessagePs != value)
                {
                    _logMessagePs = value;
                    _visibilityLog = "Visible";
                    OnPropertyChanged("LogMessage");
                    OnPropertyChanged("VisibilityLog");
                }
            }

        }

        public string VisibilityLog
        {
            get { return _visibilityLog; }
            set
            {
                if (_visibilityLog != value)
                {
                    _visibilityLog = value;
                    OnPropertyChanged("VisibilityLog");
                }
            }
        }

        private void ClearMessage()
        {
            _logMessagePs = string.Empty;
            _visibilityLog = "Collapsed";
            OnPropertyChanged("LogMessage");
            OnPropertyChanged("VisibilityLog");
        }

    }
}
