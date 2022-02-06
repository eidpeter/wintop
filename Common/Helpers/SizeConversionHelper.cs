using System;

namespace wintop.Common.Helpers
{
    public static class SizeConversionHelper
    {
        public static double BytesToGB(long bytes)
        {
            return bytes / (1024 * 1024 * 1024);
        }

        public static double BytesToMB(long bytes)
        {
            return bytes / (1024 * 1024);
        }

        public static double BytesToKB(long bytes)
        {
            return bytes / 1024;
        }

        public static double KBToGB(long bytes)
        {
            return bytes / (1024 * 1024);
        }

    }
}