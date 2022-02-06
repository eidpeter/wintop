using System.Collections.Generic;
using System.Linq;

namespace wintop.Core
{
    public class Disks
    {
        public List<DiskDetails> AllDisks { get; set; } = new List<DiskDetails>();

        public long BytesWritten => AllDisks.Sum(disk => disk.BytesWritten);

        public long BytesRead => AllDisks.Sum(disk => disk.BytesRead);

    }
}