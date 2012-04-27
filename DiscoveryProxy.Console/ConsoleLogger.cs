namespace DiscoveryProxy.Console
{
    using System;

    internal class ConsoleLogger : ILogger
    {
        private readonly object _lock = new object();

        public void Log(string entry, LogLevel level)
        {
            lock (_lock)
            { 
                System.Console.WriteLine("[{0}] ({1}) {2}", level, DateTime.Now, entry);
            }
        }
    }
}
