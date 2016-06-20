using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using NLog;

namespace Logging
{
    public interface ILogger
    {
        string Name { get; }

        void Trace(string message, params object[] args);

        void Trace(Exception exception, string message, params object[] args);

        void Debug(string message, params object[] args);

        void Debug(Exception exception, string message, params object[] args);

        void Info(string message, params object[] args);

        void Info(Exception exception, string message, params object[] args);

        void Warn(string message, params object[] args);

        void Warn(Exception exception, string message, params object[] args);

        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Error", Justification = "It's correct name")]
        void Error(string message, params object[] args);

        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Error", Justification = "It's correct name")]
        void Error(Exception exception, string message, params object[] args);

        void Fatal(string message, params object[] args);

        void Fatal(Exception exception, string message, params object[] args);

        ////ObservableCollection<LogEventInfo> LogEntries { get; set; }
    }
}