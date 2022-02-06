using System.Collections.Generic;
using System.Linq;

namespace wintop.Core
{
    public class Processes
    {
        public List<ProcessDetails> AllProcesses { get; set; } = new List<ProcessDetails>();

        public int Count => AllProcesses.Count;

        public IEnumerable<ProcessDetails> OrderByDescendingCPUUsage()
        {
            return AllProcesses.OrderByDescending(p => p.ProcessorTime);
        }

        public IEnumerable<ProcessDetails> OrderByDescendingMemoryUsage()
        {
            return AllProcesses.OrderByDescending(p => p.MemoryUsage);
        }
    }
}