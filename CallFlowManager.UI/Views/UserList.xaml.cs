using CallFlowManager.UI.ViewModels.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CallFlowManager.UI.Views
{
    /// <summary>
    /// Interaction logic for Users.xaml
    /// </summary>
    public partial class UserList_TODO_DELETE : UserControl
    {
        public UserList_TODO_DELETE()
        {
            InitializeComponent();
            var context = new UserListViewModel();
            DataContext = context;
            Window parentWindow = Application.Current.MainWindow;
        }
    }
}
