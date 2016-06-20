using System;
using System.Globalization;
using System.Windows.Data;

namespace CallFlowManager.UI.Converters
{
    public class NullToBooleanFalse : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null ? false: true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
