using System;
using System.Collections.ObjectModel;
using NLog;

namespace Logging
{
    public abstract class LoggerBase : ILogger
    {
        public abstract string Name { get; }

        public abstract void Log(LogLevel level, Exception exception, string message, params object[] args);

        public void Trace(string message, params object[] args)
        {
            Log(LogLevel.Trace, null, message, args);
        }

        public void Trace(Exception exception, string message, params object[] args)
        {
            Log(LogLevel.Trace, exception, message, args);
        }

        public void Debug(string message, params object[] args)
        {
            Log(LogLevel.Debug, null, message, args);
        }

        public void Debug(Exception exception, string message, params object[] args)
        {
            Log(LogLevel.Debug, exception, message, args);
        }

        public void Info(string message, params object[] args)
        {
            Log(LogLevel.Info, null, message, args);
        }

        public void Info(Exception exception, string message, params object[] args)
        {
            Log(LogLevel.Info, exception, message, args);
        }

        public void Warn(string message, params object[] args)
        {
            Log(LogLevel.Warning, null, message, args);
        }

        public void Warn(Exception exception, string message, params object[] args)
        {
            Log(LogLevel.Warning, exception, message, args);
        }

        public void Error(string message, params object[] args)
        {
            Log(LogLevel.Error, null, message, args);
        }

        public void Error(Exception exception, string message, params object[] args)
        {
            Log(LogLevel.Error, exception, message, args);
        }

        public void Fatal(string message, params object[] args)
        {
            Log(LogLevel.Fatal, null, message, args);
        }

        public void Fatal(Exception exception, string message, params object[] args)
        {
            Log(LogLevel.Fatal, exception, message, args);
        }
    }
}