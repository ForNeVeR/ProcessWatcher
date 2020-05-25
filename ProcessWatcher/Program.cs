﻿using System;
using System.Management;

namespace ProcessWatcher
{
    public static class Program
    {
        private static string GetProcessCommandLine(uint processId)
        {
            var query = new WqlObjectQuery($"select CommandLine from Win32_Process where ProcessId = {processId}");
            using (var searcher = new ManagementObjectSearcher(query))
            {
                using (var result = searcher.Get())
                {
                    foreach (var x in result)
                    {
                        return Convert.ToString(x["CommandLine"]);
                    }
                }
            }

            return "[NOT DETERMINED]";
        }

        private static ManagementEventWatcher CreateWatcher()
        {
            var watcher = new ManagementEventWatcher(new WqlEventQuery("select * from Win32_ProcessStartTrace"));
            watcher.EventArrived += (_, e) =>
            {
                var properties = e.NewEvent.Properties;
                var processId = Convert.ToUInt32(properties["ProcessID"].Value);
                var processName = Convert.ToString(properties["ProcessName"].Value);
                var commandLine = GetProcessCommandLine(processId);
                Console.WriteLine($"Process {processId}: {processName} {commandLine}");
            };
            return watcher;
        }

        public static void Main()
        {
            using (var watcher = CreateWatcher())
            {
                Console.WriteLine("Starting the watch. Press Enter to exit");
                watcher.Start();
                Console.ReadLine();
            }
        }
    }
}