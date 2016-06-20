using System.IO.Packaging;
using System.Windows;
using CallFlowManager.UI.ViewModels;

namespace CallFlowManager.UI.Views
{
    /// <summary>
    /// Interaction logic for OpenFileDialog.xaml
    /// </summary>
    public partial class OpenFileDialogWindow : Window
    {
        public OpenFileDialogWindow(string message)
        {
            InitializeComponent();
            var context = new OpenFileViewModel(message);
            DataContext = context;
        }
    }
}
