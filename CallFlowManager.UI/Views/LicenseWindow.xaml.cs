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
using System.Windows.Shapes;
using CallFlowManager.UI.ViewModels.License;

namespace CallFlowManager.UI.Views
{
    /// <summary>
    /// Interaction logic for LicenseWindow.xaml
    /// </summary>
    public partial class LicenseWindow : Window
    {
        //public LicenseWindow(string message)
        //{
        //    InitializeComponent();
        //    var context = new LicenseViewModel(message);
        //    DataContext = context;
        //    context.RequestCloseDialogEvent += (sender, args) => this.Close();
        //}

        public LicenseWindow()
        {
            InitializeComponent();
        }
    }
}
