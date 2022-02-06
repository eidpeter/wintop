using System;
using System.Runtime.InteropServices;

namespace wintop.Core.Windows
{
    internal class CPUUtilization
    {
        private struct ProcessCPUInformation
        {
            public long idleTime;
            public long kernelTime;
            public long userTime;
        }

        private ProcessCPUInformation processCPUInfo = new ProcessCPUInformation();

        public int CurrentUtilization
        {
            get
            {
                if (!Kernel32.GetSystemTimes(out var idleTime, out var kernelTime, out var userTime))
                {
                    int error = Marshal.GetLastWin32Error();
                    var exception = new OutOfMemoryException();
                    throw exception;
                }

                long cpuTotalTime = ((long)userTime - processCPUInfo.userTime) + ((long)kernelTime - processCPUInfo.kernelTime);

                long cpuBusyTime = cpuTotalTime - ((long)idleTime - processCPUInfo.idleTime);

                processCPUInfo.kernelTime = (long)kernelTime;
                processCPUInfo.userTime = (long)userTime;
                processCPUInfo.idleTime = (long)idleTime;

                if (cpuTotalTime > 0 && cpuBusyTime > 0)
                {
                    long reading = cpuBusyTime * 100 / cpuTotalTime;
                    reading = Math.Min(reading, 100);
                    return (int)reading;
                }

                return 0;
            }
        }


    }
}