using System.Collections.Generic;
using System.Management;
using System.Threading.Tasks;

namespace wintop.Core.Windows
{
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public class WMIReader
    {
        public Task<ManagementBaseObject> ExecuteScalar(string query)
        {
            var tsc = new TaskCompletionSource<ManagementBaseObject>();
            using (var searcher = new ManagementObjectSearcher(query))
            {
                ManagementOperationObserver results = new ManagementOperationObserver();

                results.ObjectReady += (s, e) =>
                {
                    tsc.SetResult(e.NewObject);
                };

                searcher.Get(results);

                return tsc.Task;
            }
        }

        public Task<List<ManagementBaseObject>> Execute(string query)
        {
            var tsc = new TaskCompletionSource<List<ManagementBaseObject>>();
            var items = new List<ManagementBaseObject>();

            using (var searcher = new ManagementObjectSearcher(query))
            {
                ManagementOperationObserver results = new ManagementOperationObserver();

                results.ObjectReady += (s, e) =>
                {
                    items.Add(e.NewObject);
                };

                results.Completed += (s, e) =>
                {
                    tsc.SetResult(items);
                };

                searcher.Get(results);

                return tsc.Task;
            }
        }
    }
}