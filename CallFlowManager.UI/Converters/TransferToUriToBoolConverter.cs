using System;
using System.Globalization;
using System.Windows.Data;

namespace CallFlowManager.UI.Converters
{
    [ValueConversion(typeof(string), typeof(bool))]
    class TransferToUriToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string valueAsString = (string)value;
            if (valueAsString == "TransferToUri" | valueAsString == "TransferToVoicemailUri" | valueAsString == "TransferToPstn")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
