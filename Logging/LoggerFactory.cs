using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Logging
{
    public class LoggerFactory : ILoggerFactory
    {
        private static readonly Lazy<ILoggerFactory> InstanceFactory = new Lazy<ILoggerFactory>(() => new LoggerFactory(), true);

        private readonly IDictionary<string, ILogger> _typeLoggers = new Dictionary<string, ILogger>();
        private readonly object _loggersLock = new object();

        public static ILoggerFactory Instance
        {
            get { return InstanceFactory.Value; }
        }

        public ILogger GetLogger(string name)
        {
            return GetLoggerInternal(name);
        }

        public ILogger GetLogger(Type type)
        {
            return GetLoggerInternal(type.FullName);
        }

        public ILogger GetCurrentClassLogger()
        {
            var frame = new StackFrame(1, false);
            return GetLogger(frame.GetMethod().DeclaringType);
        }

        public void Flush()
        {
            NLog.LogManager.Flush();
        }

        private ILogger GetLoggerInternal(string fullTypeName)
        {
            var key = fullTypeName.ToUpperInvariant();

            lock (_loggersLock)
            {
                ILogger logger;
                if (!_typeLoggers.TryGetValue(key, out logger))
                {
                    logger = new NLogger(NLog.LogManager.GetLogger(fullTypeName));
                    _typeLoggers.Add(key, logger);
                }

                return logger;
            }
        }
    }
}