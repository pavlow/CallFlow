using System;
using System.Windows.Controls;
using CallFlowManager.UI.ViewModels.Holidays;
using System.Windows;

namespace CallFlowManager.UI.Views
{
    /// <summary>
    /// Interaction logic for Holidays.xaml
    /// </summary>
    public partial class Holidays : UserControl
    {
        private static Logging_Old log;
        public Holidays()
        {
            log = Logging_Old.Instance;

            try
            {
                InitializeComponent();
                var context = new HolidaysViewModel();
                DataContext = context;
                Window parentWindow = Application.Current.MainWindow;
                parentWindow.Loaded += delegate { context.LoadCommand.Execute(null); };
            }
            catch (Exception ex)
            {
                log.WriteToLog(ex.ToString());
                throw;
            }
            
        }
    }
}
