namespace ProcessWatcher
{
    public struct ProcessInfo
    {
        public uint ProcessId;
        public string ProcessName;
        public PreciseValue<string> CommandLine;
    }
}