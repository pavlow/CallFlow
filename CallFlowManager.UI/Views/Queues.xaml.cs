using System;
using System.Windows.Controls;
using CallFlowManager.UI.ViewModels.Queues;
using System.Windows;

namespace CallFlowManager.UI.Views
{
    /// <summary>
    /// Interaction logic for Queues.xaml
    /// </summary>
    public partial class Queues : UserControl
    {
        private static Logging_Old log;
        public Queues()
        {
            log = Logging_Old.Instance;

            try
            {
                InitializeComponent();
                var context = new QueuesViewModel();
                DataContext = context;

                //Start loading PS Queues
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
