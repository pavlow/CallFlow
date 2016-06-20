using System;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Logging
{
    public interface ILoggerFactory
    {
        ILogger GetLogger(string name);

        ILogger GetLogger(Type type);

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "It's ok")]
        ILogger GetCurrentClassLogger();

        void Flush();
    }
}