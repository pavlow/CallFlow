using System;

namespace CallFlowManager.UI.Common
{
    public static class TextHelper
    {
        public static string ConvertSpaceToPlus(string line)
        {
            string result = string.Empty;
            if (line != null)
            {
                result = String.Join("+", line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            }
            return result;
        }
    }
}
