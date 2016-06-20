using System;
using System.Windows.Controls;
using CallFlowManager.UI.Common;
using CallFlowManager.UI.ViewModels.Logs;

namespace CallFlowManager.UI.Views
{
    /// <summary>
    /// Interaction logic for Logs.xaml
    /// </summary>
    public partial class Logs : UserControl
    {
        private static readonly NlogMemTarget _targetLogTable = new NlogMemTarget("LogTable", NLog.LogLevel.Trace, "*", "||", true, true, true);
        LogsViewModel context = new LogsViewModel();

        public Logs()
        {
            DataContext = context;
            InitializeComponent();
            
            _targetLogTable.Log += log => LogText(log);
            //this.Loaded += (s, e) =>
            //{
            //    _targetLogTable.Log += log => LogText(log);
            //};
        }

        private void LogText(string message)
        {
            this.Dispatcher.Invoke((Action)delegate()
            {
                context.AddEntry(message);
            });
        }
        
    }
}
