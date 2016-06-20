using System;
using System.Windows.Controls;
using CallFlowManager.UI.ViewModels;
using CallFlowManager.UI.ViewModels.BusinessHours;
using System.Windows;
using CallFlowManager.UI.ViewModels.Numbers;

namespace CallFlowManager.UI.Views
{
    /// <summary>
    /// Interaction logic for BusinessHours.xaml
    /// </summary>
    public partial class Numbers : UserControl
    {
        private static Logging_Old log;
        public Numbers()
        {
            log = Logging_Old.Instance;

            try
            {
                InitializeComponent();
                var numbersViewModel = new NumbersViewModel();
                DataContext = numbersViewModel;

                Window parentWindow = Application.Current.MainWindow;
                parentWindow.Loaded += delegate { numbersViewModel.LoadCommand.Execute(null); };
            }
            catch (Exception ex)
            {
                log.WriteToLog(ex.ToString());
                throw;
            }
        }
    }
}
