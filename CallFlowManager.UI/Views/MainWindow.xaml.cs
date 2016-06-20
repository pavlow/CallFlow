using System;
using System.Windows.Threading;
using CallFlowManager.UI.Common;
using CallFlowManager.UI.ViewModels;
using MahApps.Metro.Controls;
using Logging;

namespace CallFlowManager.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow 
    {
        private MainWindowViewModel context;
        private static readonly NlogMemPsTarget _target = new NlogMemPsTarget("PsLogPopup", NLog.LogLevel.Warn, "*"); 
        private static readonly NLogger Logger = LoggerFactory.Instance.GetCurrentClassLogger() as NLogger;

        private static Logging_Old log;

        public MainWindow()
        {
            context = MainWindowViewModel.Instance;
            try
            {
                InitializeComponent();
                if (!License.Validate())
                {
                    Environment.Exit(0);
                }
                
                DataContext = context;
                _target.Log += log => LogText(log);
                //this.Loaded += (s, e) =>
                //{
                ////    _target = new NlogMemPsTarget("PsLogPopup", NLog.LogLevel.Info, "*"); 
                //    //_target = new NlogMemTarget("PsLogPopup", NLog.LogLevel.Warn, "*Ps*");
                //    _target.Log += log => LogText(log);
                //};

                Logger.Info("MainWindow started");
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex.ToString());
                log.WriteToLog(ex.ToString());
                throw;
            }
        }

        private void LogText(string message)
        {
            this.Dispatcher.Invoke((Action)delegate()
            {
                context.LogMessage += String.Concat(DateTime.Now.ToString(), "\t", message + "\n");
            });
        }
    }
}
