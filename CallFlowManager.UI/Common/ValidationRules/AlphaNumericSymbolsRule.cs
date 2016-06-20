using System;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace CallFlowManager.UI.Common.ValidationRules
{
    class AlphaNumericSymbolsRule : ValidationRule
    {
        private static readonly Regex AlphaNumericSymbols = new Regex(@"^[a-zA-Z0-9_ \-\+]*$");

        public string Message { get; set; }

        public AlphaNumericSymbolsRule()
        {
            Message = "Illegal characters";
        }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            var content = value as String; 
            
            if (content != null)
            {
                if (!AlphaNumericSymbols.Match(content).Success)
                {
                    return new ValidationResult(false, Message);
                }
            }

            return new ValidationResult(true, null);
        }
    }
}
