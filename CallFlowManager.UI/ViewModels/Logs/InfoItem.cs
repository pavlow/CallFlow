using CallFlowManager.UI.Common;

namespace CallFlowManager.UI.ViewModels.Logs
{
    public class InfoItem : PropertyChangedBase
    {
        public InfoItem(string longdate, string message, string logger, string level)
        {
            DateItem = longdate;
            Message = message;
            Logger = logger;
            Level = level;
        }
        public string DateItem { get; set; }
        public string Message { get; set; }
        public string Logger { get; set; }
        public string Level { get; set; }
    }
}
