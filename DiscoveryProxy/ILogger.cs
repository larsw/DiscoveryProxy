using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscoveryProxy
{
    public interface ILogger
    {
        void Log(string entry, LogLevel level);
    }

    public enum LogLevel
    { 
        Info,
        Warn,
        Error,
        Debug,
        Verbose = Info + Warn + Error + Debug
    }

    public class NullLogger : ILogger
    {
        public void Log(string entry, LogLevel level)
        {
        }
    }
}
