using System;
using SD = System.Diagnostics;
using System.Runtime.InteropServices;
using wintop.Common.Helpers;

namespace wintop.Core.Windows
{
    internal class ProcessWrapper
    {
        private SD.Process process;

        public string Name { get; set; }

        public int PID { get; set; }

        public string Owner { get; set; }

        public string FileName { get; set; }

        public int ThreadCount { get; set; }

        public double CPUUsage { get; set; } = 0;

        public long MemoryUsage { get; set; }

        private TimeSpan totalProcessorTime;

        private SD.Stopwatch stopwatch;

        public ProcessWrapper(SD.Process Process)
        {
            process = Process;
            ParseProcess();
            OnCreated();
        }

        private void ParseProcess()
        {

            Name = process.ProcessName;
            PID = process.Id;
            ThreadCount = process.Threads.Count;
            MemoryUsage = process.WorkingSet64;

            try
            {
                FileName = process.MainModule.FileName;
                Owner = GetOwner();
            }
            catch
            {
                FileName = "";
                Owner = "SYSTEM";
            }
        }

        private void OnCreated()
        {
            // Does not work for system processes with managed code
            if (Owner != "SYSTEM")
            {
                stopwatch = new SD.Stopwatch();
                stopwatch.Start();
                totalProcessorTime = process.TotalProcessorTime;
            }
        }

        private string GetOwner()
        {
            string owner = string.Empty;

            if (WtsApi32.WTSQuerySessionInformation(IntPtr.Zero, process.SessionId, WtsApi32.WtsInfoClass.WTSUserName, out IntPtr buffer, out int strLen) && strLen > 1)
            {
                owner = Marshal.PtrToStringAnsi(buffer);
                WtsApi32.WTSFreeMemory(buffer);
            }

            return owner;
        }

        public void Refresh()
        {

            if (Owner != "SYSTEM")
            {
                stopwatch.Stop();

                var newTotalProcessorTime = process.TotalProcessorTime;
                var cpuUsedMilliseconds = (newTotalProcessorTime - totalProcessorTime).TotalMilliseconds;

                var cpuUsageTotal = cpuUsedMilliseconds / (Environment.ProcessorCount * stopwatch.ElapsedMilliseconds);

                CPUUsage = Math.Round(cpuUsageTotal * 100, 2);

                totalProcessorTime = newTotalProcessorTime;

                stopwatch.Restart();
            }
            MemoryUsage = process.WorkingSet64;
        }


        public override string ToString()
        {
            return $"{CPUUsage} %, {Name} {MemoryUsage} Mb (owner: {Owner})";
        }

    }
}