using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CallFlowManager.UI.Models;
using System.Windows.Input;
using CallFlowManager.UI.Properties;

namespace CallFlowManager.UI.ViewModels.License
{
    using Common;

    public class LicenseViewModel : PropertyChangedBase
    {
        public event EventHandler RequestCloseDialogEvent = delegate { };

        public event EventHandler DialogResultTrueEvent = delegate { };

        private ConfigService _config = ConfigService.Instance;
        private string _message;
        private string _statusBar;
        private bool _isVisible;
        private string _softwareTitle;
        public ObservableCollection<string> SoftwareTitles { get; private set; }

        public LicenseViewModel()
        {
            OkCommand = new RelayCommand(_ => VerifyLicense());
            ExitCommand = new RelayCommand(_ => Environment.Exit(0));

            _message = "Please validate your license by completing the following information:";
            if (_config.Config.IsActive)
            {
                _statusBar = Resources.ValidationWasSuccessful;
            }
            else
            {
                _statusBar = Resources.ActivationWasUnsuccessful;
            }

            SoftwareTitles = Globals.SoftwareTitles;
        }

        public ICommand OkCommand { get; private set; }

        public ICommand ExitCommand { get; private set; }

        public string Message
        {
            get { return _message; }
            set
            {
                if (value != _message)
                {
                    _message = value;
                    OnPropertyChanged("Message");
                }
            }
        }

        public string StatusBar
        {
            get { return _statusBar; }
            set
            {
                if (value != _statusBar)
                {
                    _statusBar = value;
                    OnPropertyChanged("StatusBar");
                }
            }
        }

        public bool IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (value != _isVisible)
                {
                    _isVisible = value;
                    OnPropertyChanged("IsVisible");
                }
            }
        }


        private bool _isLoading;

        public bool IsLoading
        {
            get { return _isLoading; }
            set
            {
                if (_isLoading != value)
                {
                    _isLoading = value;

                    OnPropertyChanged("IsLoading");
                }
            }
        }


        public string SoftwareTitle
        {
            get { return _softwareTitle; }
            set
            {
                _softwareTitle = value;
                OnPropertyChanged("SoftwareTitle");
            }
        }


        public ConfigService WfConfig
        {
            get { return _config; }
            set
            {
                if (value != _config)
                {
                    _config = value;
                    OnPropertyChanged("WfConfig");
                }
            }
        }

        public async Task VerifyLicense()
        {
            bool isValid;

            await Task.Run(
                () =>
                {
                    StatusBar = Resources.Checking;
                    IsLoading = true;

                    if (!_config.Config.IsActive)
                    {
                        // Check activation
                        isValid = License.Request("activation") || License.Request("status");
                        _config.Config.IsActive = isValid;
                    }
                    else
                    // Check status
                    {
                        isValid = License.Request("status");
                    }

                    StatusBar = !isValid ? Resources.ActivationWasUnsuccessful : Resources.ValidationWasSuccessful;

                    IsLoading = false;
                    License.SaveConfig();

                    if (isValid)
                    {
                        Thread.Sleep(1000);
                        DialogResultTrueEvent(this, EventArgs.Empty);
                        RequestCloseDialogEvent(this, EventArgs.Empty);
                    }
                });
        }
    }
}
