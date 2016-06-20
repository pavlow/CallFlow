using System.Globalization;
using System.Windows;
using System.Windows.Markup;
using Logging;

namespace CallFlowManager.UI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly ILogger Logger = LoggerFactory.Instance.GetCurrentClassLogger();

        /// <summary>
        ///  http://stackoverflow.com/questions/3630967/datetime-not-showing-with-currentculture-format-in-datagrid-listview
        /// </summary>
        static App()
        {
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            Logger.Error("Application started");
        }


        /// <summary>
        /// Handles unhandled application exceptions
        /// http://www.wpf-tutorial.com/wpf-application/handling-exceptions/
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            Logging_Old log = Logging_Old.Instance;
            Logger.Fatal(e.Exception, "An unhandled exception just occurred: ");
            log.WriteToLog(e.Exception.Message);
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message, "Unhandled Exception, Sorry!", MessageBoxButton.OK, MessageBoxImage.Error);
            
            //This tells WPF that we're done dealing with this exception and nothing else should be done about it:
            e.Handled = true;
        }

    }
}
