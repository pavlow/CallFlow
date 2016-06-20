using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using CallFlowManager.UI.Common;
using CallFlowManager.UI.Models;

namespace CallFlowManager.UI.ViewModels
{
    public class OpenFileViewModel : PropertyChangedBase
    {

        private string _message;
        private LicenseData _currentLicense;

        public OpenFileViewModel(string message)
        {
            //OpenLicenseCommand = new RelayCommand(_ => OpenLicense());
            //ExitCommand = new RelayCommand(_ => Environment.Exit(0));
            //_message = message;
        }

        //public string Message
        //{
        //    get { return _message; }
        //    set
        //    {
        //        if (value != _message)
        //        {
        //            _message = value;
        //            OnPropertyChanged("Message");
        //        }
        //    }
        //}

        //public ICommand OpenLicenseCommand { get; private set; }

        //public ICommand ExitCommand { get; private set; }


        //public void OpenLicense()
        //{
        //    FileHelper.OpenLicenseFile(Path.Combine(License.LicenseFolder, "license.lic"));

        //    // Close window
        //    Application.Current.Windows.Cast<Window>().Single(w => w.DataContext == this).Close();

        //}

    }
}
