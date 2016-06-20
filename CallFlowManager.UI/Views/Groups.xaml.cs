using System;
using System.Windows.Controls;
using CallFlowManager.UI.ViewModels.Groups;
using System.Windows;
using System.Windows.Data;
using Logging;

namespace CallFlowManager.UI.Views
{
    /// <summary>
    /// Interaction logic for Groups.xaml
    /// </summary>
    public partial class Groups : UserControl
    {
        private static readonly NLogger Logger = LoggerFactory.Instance.GetCurrentClassLogger() as NLogger;
        private static Logging_Old log;
        public Groups()
        {
            log = Logging_Old.Instance;

            try
            {
                InitializeComponent();
                var context = new GroupsViewModel();
                DataContext = context;
                Window parentWindow = Application.Current.MainWindow;
                parentWindow.Loaded += delegate { context.LoadCommand.Execute(null); };
                //this.Loaded += delegate
                //{
                //    BindingExpression be = displayName.GetBindingExpression(TextBox.TextProperty);
                //    be.UpdateSource();
                //};
                //this.Loaded += (s, e) =>
                //{
                ////    _target = new NlogMemPsTarget("PsLogPopup", NLog.LogLevel.Info, "*"); 
                //    //_target = new NlogMemTarget("PsLogPopup", NLog.LogLevel.Warn, "*Ps*");
                //    _target.Log += log => LogText(log);
                //};
            }
            catch (Exception ex)
            {
                log.WriteToLog(ex.ToString());
                throw;
            }
        }
    }
}
