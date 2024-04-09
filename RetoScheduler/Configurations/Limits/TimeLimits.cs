using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetoScheduler.Configurations.Limits
{
    public class TimeLimits
    {
        public TimeLimits(TimeOnly startTime, TimeOnly endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }
        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }
    }
}
