using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetoScheduler.Configurations
{
    public class TimeLimits
    {
        public TimeLimits(OnlyTime startTime, OnlyTime? endTime = null)
        {
            StartTime = startTime;
            EndDate = endTime;
        }
        public OnlyTime StartTime { get; set; }

        public OnlyTime? EndDate { get; set; }
    }
}
