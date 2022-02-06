using System;

namespace wintop.Core
{
    public class SystemTime
    {
        private DateTime now;

        public SystemTime(DateTime Now)
        {
            now = Now;
        }

        public override string ToString()
        {
            return now.ToString("dddd, dd MMMM yyyy HH:mm:ss");
        }
    }
}