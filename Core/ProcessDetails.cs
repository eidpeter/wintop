using System.ComponentModel;

namespace wintop.Core
{
    public class ProcessDetails
    {
        public string Name { get; set; }

        public int PID { get; set; }

        [DisplayName("CPU (%)")]
        public double ProcessorTime { get; set; }

        [DisplayName("Memory (MB)")]
        public long MemoryUsage { get; set; }

        [DisplayName("Thread Count")]
        public int ThreadCount { get; set; }

        [DisplayName("User")]
        public string Owner { get; set; }
    }
}