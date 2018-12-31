using System;
using System.Management;

namespace ProcessWatcher
{
    public static class Program
    {
        private static ManagementEventWatcher CreateWatcher()
        {
            var watcher = new ManagementEventWatcher(new WqlEventQuery("select * from Win32_ProcessStartTrace"));
            watcher.EventArrived += (_, e) =>
            {
                Console.WriteLine($"Process: {e.NewEvent.Properties["ProcessName"].Value}");
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