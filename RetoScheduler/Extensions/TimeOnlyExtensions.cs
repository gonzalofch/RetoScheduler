using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetoScheduler.Extensions
{
    public static class TimeOnlyExtensions
    {
        public static TimeOnly AddSeconds(this TimeOnly time, double seconds)
        {
            var ticks = (long)(seconds * 10000000 + (seconds >= 0 ? 0.5 : -0.5));
            return AddTicks(time, ticks);
        }
        public static TimeOnly AddTicks(this TimeOnly time, long ticks)
        {
            return new TimeOnly(time.Ticks + ticks);
        }
    }
}
