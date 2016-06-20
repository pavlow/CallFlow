using System.Windows;
using System.Windows.Controls;

namespace CallFlowManager.UI.Common
{
    class LabelExtension
    {
        // Using a DependencyProperty as the backing store for IsRequired.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsRequiredProperty =
            DependencyProperty.Register("IsRequired", typeof(bool), typeof(Label), new PropertyMetadata(false));
    }
}
