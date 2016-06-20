using System;
using System.Runtime.Remoting.Channels;
using System.Windows.Controls;
using CallFlowManager.UI.ViewModels;
using CallFlowManager.UI.ViewModels.BusinessHours;
using System.Windows;

namespace CallFlowManager.UI.Views
{
    /// <summary>
    /// Interaction logic for BusinessHours.xaml
    /// </summary>
    public partial class BusinessHours : UserControl
    {
        private static Logging_Old log;
        public BusinessHours()
        {
            log = Logging_Old.Instance;

            try
            {
                InitializeComponent();
                var businessHoursViewModel = new BusinessHoursViewModel();
                DataContext = businessHoursViewModel;
                
                Window parentWindow = Application.Current.MainWindow;
                parentWindow.Loaded += delegate { businessHoursViewModel.LoadCommand.Execute(null); };
            }
            catch (Exception ex)
            {
                log.WriteToLog(ex.ToString());
                throw;
            }
        }
    }
}
