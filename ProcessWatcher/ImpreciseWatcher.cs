using System;
using System.Management;

namespace ProcessWatcher
{
    /// <summary>This watcher may skip some events.</summary>
    public static class ImpreciseWatcher
    {
        public static IDisposable StartNew(Action<ProcessInfo> handler)
        {
            var watcher = new ManagementEventWatcher(
                new WqlEventQuery(@"select * from __InstanceCreationEvent within 1 where TargetInstance isa 'Win32_Process'"));

            watcher.EventArrived += (_, e) =>
            {
                var process = (ManagementBaseObject) e.NewEvent["TargetInstance"];
                var processId = Convert.ToUInt32(process.Properties["ProcessID"].Value);
                var processName = Convert.ToString(process.Properties["Name"].Value);
                var commandLine = Convert.ToString(process.Properties["CommandLine"].Value);
                var processInfo = new ProcessInfo
                {
                    ProcessId = processId,
                    ProcessName = processName!,
                    CommandLine = PreciseValue.Precise(commandLine)
                };
                handler(processInfo);
            };
            
            watcher.Start();
            return watcher;
        }
    }
}