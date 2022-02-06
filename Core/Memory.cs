using System;

namespace wintop.Core
{
    public class Memory
    {
        public long PhysicalMemoryTotal { get; set; }

        public long PhysicalMemoryAvailable { get; set; }

        public int PhysicalMemoryUsed { get; set; }

        // public int PhysicalMemoryTotal { get; set; }

        // public int PhysicalMemoryAvailable { get; set; }

        // public int PhysicalMemoryUsed { get; set; }

        // public int SwapMemoryTotal { get; set; }

        // public int SwapMemoryAvailable { get; set; }

        // public int SwapMemoryUsed => (int)Math.Round((decimal)((SwapMemoryTotal - SwapMemoryAvailable) * 100) / SwapMemoryTotal, 0);
    }
}