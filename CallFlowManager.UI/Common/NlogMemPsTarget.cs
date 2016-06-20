using System;
using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Targets;

namespace CallFlowManager.UI.Common
{
    public class NlogMemPsTarget : Target
    {
        public Action<string> Log = delegate { };
        public bool IsLevel;
        public bool IsLoggerName;
        public string Separator = "\t";

        public NlogMemPsTarget(string name, LogLevel level, string rule, bool isLevel = false, bool isLoggerName = false)
        {
            LogManager.Configuration.AddTarget(name, this);
            LogManager.Configuration.LoggingRules.Add(new LoggingRule(rule, level, this));//This will ensure that exsiting rules are not overwritten
            LogManager.Configuration.Reload(); //This is important statement to reload all applied settings
            NameTarget = name;
            LevelTarget = level.Name;
            IsLevel = isLevel;
            IsLoggerName = isLoggerName;
        }

        public string NameTarget { get; set; }

        public string LevelTarget { get; set; }

        protected override void Write(AsyncLogEventInfo[] logEvents)
        {
            foreach (var logEvent in logEvents)
            {
                Write(logEvent);
            }
        }

        protected override void Write(AsyncLogEventInfo logEvent)
        {
            Write(logEvent.LogEvent);
        }

        protected override void Write(LogEventInfo logEvent)
        {
            string layout = logEvent.TimeStamp + Separator;

            layout += logEvent.FormattedMessage + Separator;
            
            Log(layout);
        }
    }
}
