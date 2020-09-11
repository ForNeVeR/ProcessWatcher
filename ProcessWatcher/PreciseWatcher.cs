using System;
using System.Management;

namespace ProcessWatcher
{
    /// <summary>
    /// Precise watcher always detects processes being started, but isn't always able to detect their command line.
    /// </summary>
    public static class PreciseWatcher
    {
        private static string? GetProcessCommandLine(uint processId)
        {
            var query = new WqlObjectQuery($"select CommandLine from Win32_Process where ProcessId = {processId}");
            using var searcher = new ManagementObjectSearcher(query);
            using var result = searcher.Get();
            
            foreach (var x in result)
            {
                return Convert.ToString(x["CommandLine"]);
            }

            return null;
        }
        
        public static IDisposable StartNew(Action<ProcessInfo> handler)
        {
            var watcher = new ManagementEventWatcher(
                new WqlEventQuery("select * from Win32_ProcessStartTrace"));
            watcher.EventArrived += (_, e) =>
            {
                var properties = e.NewEvent.Properties;
                var processId = Convert.ToUInt32(properties["ProcessID"].Value);
                var commandLine = GetProcessCommandLine(processId);
                
                var processName = Convert.ToString(properties["ProcessName"].Value);
                var processInfo = new ProcessInfo
                {
                    ProcessId = processId,
                    ProcessName = processName!,
                    CommandLine = PreciseValue.Imprecise(commandLine)
                };
                handler(processInfo);
            };
            
            watcher.Start();
            return watcher;
        }
    }
}