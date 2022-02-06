using System.Collections.Generic;
using System.Threading.Tasks;

namespace wintop.Core
{
    public interface ISystemInfo
    {
        int CPUCount { get; }

        int DiskCount { get; }

        Task<DeviceInformation> GetDeviceInformation();

        Task<CPU> GetCPUUsage();

        Task<Memory> GetMemoryUsage();

        Task<Disks> GetDisksUsage();

        Task<IEnumerable<Storage>> GetStorageUsage();

        Task<SystemTime> GetSystemTime();

        Task<Network> GetNetworkUsage();

        Task<Processes> GetProcesses();
    }
}