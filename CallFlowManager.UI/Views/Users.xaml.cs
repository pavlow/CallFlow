using System;
using CallFlowManager.UI.ViewModels.Users;
using System.Windows;
using System.Windows.Controls;


namespace CallFlowManager.UI.Views
{
    /// <summary>
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class Users : UserControl
    {
        private static Logging_Old log;
        public Users()
        {
            log = Logging_Old.Instance;

            try
            {
                InitializeComponent();
                var usersViewModel = new UsersViewModel();
                DataContext = usersViewModel;
                Window parentWindow = Application.Current.MainWindow;
                parentWindow.Loaded += delegate { usersViewModel.LoadCommand.Execute(null); };
            }
            catch (Exception ex)
            {
                log.WriteToLog(ex.ToString());
                throw;
            }
        }
    }
}
