using System;
using System.Windows.Controls;

namespace CallFlowManager.UI.Common.ValidationRules
{
    class EmailRule : ValidationRule
    {
        private const string Message1 = "Input the string in e-mail format";
        private const string Message2 = "String can't be empty";

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            var content = value as String;

            if (String.IsNullOrEmpty(content))
            {
                return new ValidationResult(false, Message2);
            }

            bool response = ValidateHelper.IsValidEmail(content);
            return !response ? new ValidationResult(false, Message1) : new ValidationResult(true, null);
        }
    }
}

