using System.Collections.Generic;
using System.Linq;
using wintop.Core;

namespace wintop.Widgets
{
    public class OrderProcessesByPID : ProcessOrder
    {
        protected override IEnumerable<ProcessDetails> OrderDescending(Processes processes)
        {
            return processes.AllProcesses.OrderByDescending(p => p.PID);
        }

        protected override IEnumerable<ProcessDetails> OrderAscending(Processes processes)
        {
            return processes.AllProcesses.OrderBy(p => p.PID);
        }

    }
}