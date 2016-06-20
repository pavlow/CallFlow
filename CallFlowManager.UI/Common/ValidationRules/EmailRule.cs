using System;
using System.Net.Mail;
using System.Windows.Controls;

namespace CallFlowManager.UI.Common.ValidationRules
{
    class EmailRule : ValidationRule
    {
        private const string Message1 = "Input the string in e-mail format";
        private const string Message2 = "String can't be empty";

        public bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            var content = value as String;

            if (content != null)
            {
                if (!IsValid(content))
                {
                    return new ValidationResult(false, Message1);
                }
            }
            if (String.IsNullOrEmpty(content))
            {
                return new ValidationResult(false, Message2);
            }

            return new ValidationResult(true, null);
        }
    }
}

