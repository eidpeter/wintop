using System;

namespace wintop.Core
{
    public class Storage
    {
        public string Name { get; set; }

        public string DriveFormat { get; set; }

        public string DriveType { get; set; }

        public string VolumeLabel { get; set; }

        public long AvailableFreeSpace { get; set; }

        public long TotalSize { get; set; }

        public long UsedSpace => TotalSize - AvailableFreeSpace;

        public int PercentageUsed => (int)Math.Floor((decimal)(UsedSpace *100 / TotalSize));
    }
}