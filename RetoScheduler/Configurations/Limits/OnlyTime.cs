using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetoScheduler.Configurations.Limits
{
    public class OnlyTime
    {
        public OnlyTime(int hours, int minutes, int seconds)
        {
            DailyTimeSpan = new TimeSpan(hours, minutes, seconds);
        }
        public TimeSpan DailyTimeSpan { get; set; } //accedo a esta propiedad para obtener el tiempo en formato 20:00:00
    }
}
