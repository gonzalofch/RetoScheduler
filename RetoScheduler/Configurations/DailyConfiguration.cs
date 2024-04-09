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
        public DailyConfiguration(DailyConfigType type, TimeOnly? onceAt, int? frecuency, DailyFrecuency? dailyFrecuencyType, TimeLimits? timeLimits)
        {

            Type = type;
            OnceAt = (type == DailyConfigType.Once && onceAt.HasValue) ? onceAt.Value : null;
            Frecuency = (type == DailyConfigType.Recurring) ? frecuency : null;
            DailyFrecuencyType = (type == DailyConfigType.Recurring) ? dailyFrecuencyType : null;
            TimeLimits = (type == DailyConfigType.Recurring) ? timeLimits : null;
        }

        public DailyConfigType Type { get; set; } //once | recurring ( 1 day )

        public TimeOnly? OnceAt { get; set; }

        public int? Frecuency { get; set; } //just the number

        public DailyFrecuency? DailyFrecuencyType { get; set; } // Evey "X" Hours, minutes, etc

        public TimeLimits? TimeLimits { get; set; }
    }
}
