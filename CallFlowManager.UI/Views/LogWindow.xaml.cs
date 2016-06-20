using System;
using System.Windows;
using System.Windows.Controls;
using CallFlowManager.UI.Common;
using CallFlowManager.UI.ViewModels;
using CallFlowManager.UI.ViewModels.Logs;
using Logging;

namespace CallFlowManager.UI.Views
{
    /// <summary>
    /// Interaction logic for LogWindow.xaml
    /// </summary>
    public partial class LogWindow : UserControl
    {
//        private LogWindowUserControlViewModel context;
//        private NlogMemTarget _target;
//        private static readonly NLogger Logger = LoggerFactory.Instance.GetCurrentClassLogger() as NLogger;
        
        
        public LogWindow()
        {
            var context = MainWindowViewModel.Instance;
            try
            {
                InitializeComponent();
                DataContext = context;
            }
            catch (Exception ex)
            {
 //               Logger.Fatal(ex.ToString());
                throw;
            }
        }
    }
}
