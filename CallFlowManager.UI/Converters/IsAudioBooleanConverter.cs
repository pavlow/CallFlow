using System;
using System.Globalization;
using System.Windows.Data;

namespace CallFlowManager.UI.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    class IsAudioBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string valueAsString = (string)value;
            if (valueAsString == "None Selected" || valueAsString == "Default Music")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}


