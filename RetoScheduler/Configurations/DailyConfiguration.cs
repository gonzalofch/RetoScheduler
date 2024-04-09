using RetoScheduler.Configurations.Limits;
using RetoScheduler.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RetoScheduler.Configurations
{
    public class DailyConfiguration
    {
        public DailyConfiguration(DailyConfigType type,TimeOnly? onceAt,int frecuency,DailyFrecuency dailyFrecuencyType, TimeLimits? timeLimits)
        {

            Type = type;

            if (type == DailyConfigType.Once && onceAt.HasValue)
            {
                OnceAt = onceAt.Value;
            }
            else
            {
                Frecuency = frecuency;
                DailyFrecuencyType = dailyFrecuencyType;
                TimeLimits = timeLimits;
            }
        }

        public DailyConfigType Type { get; set; } //once | recurring ( 1 day )

        public TimeOnly OnceAt { get; set; }

        public int Frecuency { get; set; } //just the number

        public DailyFrecuency DailyFrecuencyType { get; set; } // Evey "X" Hours, minutes, etc

        public TimeLimits? TimeLimits { get; set; }
    }
}
