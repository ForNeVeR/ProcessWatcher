using System;
using System.Globalization;
using System.Management;
using System.Threading.Tasks;

namespace ProcessWatcher
{
    public static class Program
    {
        private static void ReportProperty(string name, string value, ConsoleColor valueColor = ConsoleColor.White)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"{name}: ");
            Console.ForegroundColor = valueColor;
            Console.WriteLine(value);
        }
        
        private static void ReportProperty(string name, PreciseValue<string> value, ConsoleColor valueColor = ConsoleColor.White)
        {
            var valueString = value.Value ?? "[UNKNOWN]";
            var reportedValue = value.IsPrecise ? valueString : $"(?) {valueString}";
            ReportProperty(name, reportedValue, value.IsPrecise ? valueColor : ConsoleColor.Gray);
        }

        
        private static readonly object OutputLog = new object();
        private static void ReportEvent(string eventDescription, ProcessInfo processInfo)
        {
            Task.Run(() =>
            {
                lock (OutputLog)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("---");
                    
                    ReportProperty("Event", eventDescription);
                    ReportProperty("Process id", processInfo.ProcessId.ToString(CultureInfo.InvariantCulture));
                    ReportProperty("Process name", processInfo.ProcessName);
                    ReportProperty("Command line", processInfo.CommandLine);
                    
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("---");
                    Console.WriteLine();
                }
            });
        }
        
        public static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "--help" || args[0] == "/?")
            {
                Console.WriteLine("Arguments: [--precise] | --imprecise");
                return;
            }
            
            if (args.Length == 0 || args[0] == "--precise")
            {
                Console.WriteLine("Starting watch in PRECISE mode");
                using var _ = PreciseWatcher.StartNew(pi => ReportEvent("Win32_ProcessStartTrace", pi));
            }
            else if (args.Length == 0)
            {
                Console.WriteLine("Starting watch in IMPRECISE mode");
                using var _ = ImpreciseWatcher.StartNew(pi => ReportEvent("Win32_Process creation", pi));
            }
            else
            {
                Console.WriteLine($"Unrecognized arguments: [{string.Join(' ', args)}]");
                return;
            }

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
        }
    }
}
