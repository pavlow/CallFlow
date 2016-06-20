using System.Linq;
using System.Windows;

namespace CallFlowManager.UI.Common
{
    public static class MsgBox
    {
        public static void Error(string body, string title = null, Window owner = null)
        {
            MessageBox.Show(owner ?? GetActiveWindow(), body, title, MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
        }

        private static Window GetActiveWindow()
        {
            var windows = Application.Current.Windows.Cast<Window>().ToArray();
            return windows.FirstOrDefault(p => p.IsActive) ?? windows.First();
        }
    }
}