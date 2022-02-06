using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace wintop.Core.Windows
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public class WindowsInfo : ISystemInfo
    {
        public int CPUCount { get; } = 0;

        public int DiskCount { get; } = 0;

        private CPUUtilization CPUUtilizationReader;
        private ProcessReader processReader;

        public WindowsInfo()
        {
            CPUCount = Environment.ProcessorCount;
            DiskCount = DriveInfo.GetDrives().Length;

            CPUUtilizationReader = new CPUUtilization();

            processReader = new ProcessReader();
        }


        public Task<Processes> GetProcesses()
        {
            var tsc = new TaskCompletionSource<Processes>();
            var preader = processReader.Refresh();

            var processes = new Processes()
            {
                AllProcesses = preader.Select(p => new ProcessDetails()
                {
                    Name = p.Name,
                    ProcessorTime = p.CPUUsage,
                    PID = p.PID,
                    Owner = p.Owner,
                    ThreadCount = p.ThreadCount,
                    MemoryUsage = p.MemoryUsage
                }).ToList()
            };

            tsc.SetResult(processes);
            return tsc.Task;
        }

        public Task<CPU> GetCPUUsage()
        {
            var tsc = new TaskCompletionSource<CPU>();
            var cpu = new CPU()
            {
                Name = "Total",
                UsagePercentage = CPUUtilizationReader.CurrentUtilization
            };

            tsc.SetResult(cpu);
            return tsc.Task;
        }

        public async Task<DeviceInformation> GetDeviceInformation()
        {
            var query = "SELECT caption, version, CSName FROM Win32_OperatingSystem";
            var wmiReader = new WMIReader();
            var results = await wmiReader.ExecuteScalar(query);

            return new DeviceInformation()
            {
                DeviceName = results["CSName"].ToString(),
                OSName = results["caption"].ToString(),
                OSVersion = results["version"].ToString()
            };
        }

        public async Task<Disks> GetDisksUsage()
        {
            var query = "SELECT DiskReadBytesPersec, DiskWriteBytesPersec, Name FROM Win32_PerfRawData_PerfDisk_PhysicalDisk WHERE NOT name = '_Total'";
            var wmiReader = new WMIReader();
            var results = await wmiReader.Execute(query);

            return new Disks()
            {
                AllDisks = results.Select(mo => new DiskDetails()
                {
                    Name = mo["Name"].ToString(),
                    BytesRead = Convert.ToInt64(mo["DiskReadBytesPersec"]),
                    BytesWritten = Convert.ToInt64(mo["DiskWriteBytesPersec"])
                })
                        .ToList()
            };
        }

        public Task<Memory> GetMemoryUsage()
        {
            var tsc = new TaskCompletionSource<Memory>();
            var memory = new Memory();

            Kernel32.MEMORYSTATUSEX memoryStatusEx = new Kernel32.MEMORYSTATUSEX();
            if (!Kernel32.GlobalMemoryStatusEx(memoryStatusEx))
            {
                int error = Marshal.GetLastWin32Error();
                var exception = new Win32Exception("Could not retrieve the global memory status");
                throw exception;
            }
            else
            {
                memory.PhysicalMemoryUsed = (int)memoryStatusEx.dwMemoryLoad;
                memory.PhysicalMemoryTotal = Convert.ToInt64(memoryStatusEx.ullTotalPhys);
                memory.PhysicalMemoryAvailable = Convert.ToInt64(memoryStatusEx.ullAvailPhys);
            }

            tsc.SetResult(memory);
            return tsc.Task;
        }

        public async Task<Network> GetNetworkUsage()
        {
            var query = "SELECT BytesReceivedPersec, BytesSentPersec, Name FROM Win32_PerfRawData_Tcpip_NetworkInterface";
            var wmiReader = new WMIReader();
            var results = await wmiReader.Execute(query);

            return new Network()
            {
                Interfaces = results.Select(net => new NetworkDetails()
                {
                    Name = net["Name"].ToString(),
                    BytesSent = Convert.ToInt64(net["BytesSentPersec"]),
                    BytesReceived = Convert.ToInt64(net["BytesReceivedPersec"])
                }).ToList()
            };
        }


        public Task<IEnumerable<Storage>> GetStorageUsage()
        {

            var tsc = new TaskCompletionSource<IEnumerable<Storage>>();

            var drives = DriveInfo.GetDrives();

            var drivesWrapper = drives.Select(dr => new Storage()
            {
                Name = dr.Name,
                VolumeLabel = dr.VolumeLabel,
                AvailableFreeSpace = dr.AvailableFreeSpace,
                TotalSize = dr.TotalSize
            });

            tsc.SetResult(drivesWrapper);
            return tsc.Task;
        }

        public Task<SystemTime> GetSystemTime()
        {
            var tsc = new TaskCompletionSource<SystemTime>();
            tsc.SetResult(new SystemTime(DateTime.Now));
            return tsc.Task;
        }
    }
}