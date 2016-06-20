using System;
using CallFlowManager.UI.ViewModels.WorkFlows;
using System.Windows;
using CallFlowManager.UI.Common;
using Logging;

namespace CallFlowManager.UI.Views
{
    /// <summary>
    /// Interaction logic for Designer.xaml
    /// </summary>
    public partial class Designer
    {
        private DesignerViewModel context;
        private static Logging_Old log;
  //      private NlogMemTarget _target;
  //      private static readonly NLogger Logger = LoggerFactory.Instance.GetCurrentClassLogger() as NLogger;

        public Designer()
        {
            context = new DesignerViewModel();

            try
            {
                InitializeComponent();
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
