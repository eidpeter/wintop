using System.Collections.Generic;
using System.Linq;
using wintop.Core;

namespace wintop.Widgets
{
    public abstract class ProcessOrder
    {
        protected bool IsDescending { get; set; } = false;

        protected abstract IEnumerable<ProcessDetails> OrderDescending(Processes processes);

        protected abstract IEnumerable<ProcessDetails> OrderAscending(Processes processes);

        public void ToggleOrdering()
        {
            IsDescending = !IsDescending;
        }

        public IEnumerable<ProcessDetails> OrderProcesses(Processes processes)
        {

            if (IsDescending)
            {
                return OrderDescending(processes);
            }
            else
            {
                return OrderAscending(processes);
            }
        }
    }
}