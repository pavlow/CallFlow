using System.Windows;

namespace CallFlowManager.UI.Converters
{
    public sealed class InvBooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public InvBooleanToVisibilityConverter() :
            base(Visibility.Visible, Visibility.Collapsed) { }
    }
}
