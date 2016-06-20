using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using NLog;

namespace Logging
{
    public class NLogger : LoggerBase
    {
        private readonly Logger _logger;

        public NLogger(Logger logger)
        {
            _logger = logger;
            LogEntries = new ObservableCollection<LogEventInfo>();
        }

        public override string Name
        {
            get { return _logger.Name; }
        }

        public ObservableCollection<LogEventInfo> LogEntries { get; set; }

        public override void Log(LogLevel level, Exception exception, string format, params object[] args)
        {
            NLog.LogLevel nlogLevel = NLog.LogLevel.Info;
            switch (level)
            {
                case LogLevel.Trace:
                    nlogLevel = NLog.LogLevel.Trace;
                    break;
                case LogLevel.Debug:
                    nlogLevel = NLog.LogLevel.Debug;
                    break;
                case LogLevel.Info:
                    nlogLevel = NLog.LogLevel.Info;
                    break;
                case LogLevel.Warning:
                    nlogLevel = NLog.LogLevel.Warn;
                    break;
                case LogLevel.Error:
                    nlogLevel = NLog.LogLevel.Error;
                    break;
                case LogLevel.Fatal:
                    nlogLevel = NLog.LogLevel.Fatal;
                    break;
                default:
                    _logger.Error("Unknown log level - '{0}', using default ('{1}')", level.ToString("G"), nlogLevel);
                    break;
            }

            string message;
            try
            {
                if (args.Any())
                {
                    message = string.Format(CultureInfo.InvariantCulture, format, args);
                }
                else
                {
                    message = format;
                }
            }
            catch (FormatException)
            {
                _logger.Error("Error while formatting the following message: '{0}', parameters: '{1}'", format, string.Join(" ", args));
                message = format;
            }

            var eventInfo = exception != null
                ? LogEventInfo.Create(nlogLevel, Name, exception, CultureInfo.InstalledUICulture, message)
                : LogEventInfo.Create(nlogLevel, Name, message);
            LogEntries.Add(eventInfo);
            _logger.Log(eventInfo);
        }
    }
}