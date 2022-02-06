using System.Collections.Generic;
using SD = System.Diagnostics;
using System.Linq;

namespace wintop.Core.Windows
{
    internal class ProcessReader
    {
        List<ProcessWrapper> processWrappers;

        public ProcessWrapper[] ProcessWrapper
        {
            get { return processWrappers.ToArray(); }
        }

        public ProcessReader()
        {
            FillProcessList();
        }

        void FillProcessList()
        {
            SD.Process[] processlist = SD.Process.GetProcesses();

            // First call
            processWrappers = processlist.Select(p =>
            {
                return new ProcessWrapper(p);
            }).ToList();
        }

        public ProcessWrapper[] Refresh()
        {
            SD.Process[] processlist = SD.Process.GetProcesses();

            // Remove dead processes
            var tmpList = processWrappers.Where(pd => processlist.Any(p => p.Id == pd.PID)).ToList();

            // Add new ones
            var newProcess = processlist.Where(p => !processWrappers.Any(pd => pd.PID == p.Id)).Select(p => { return new ProcessWrapper(p); }).ToList();

            tmpList.AddRange(newProcess);

            processWrappers = tmpList;

            // Refresh metric
            processWrappers.ToList().ForEach(p => p.Refresh());

            return processWrappers.ToArray();
        }

    }
}