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
        public DailyConfiguration(ConfigType type,OnlyTime onceAt,int frecuency,DailyFrecuencyType dailyFrecuencyType, TimeLimits timeLimits)
        {

            Type = type;

            if (type == ConfigType.Once)
            {
                OnceAt = onceAt;
            }
            else
            {
                Frecuency = frecuency;
                DailyFrecuencyType = dailyFrecuencyType;
                TimeLimits = timeLimits;
            }
        }

        public ConfigType Type { get; set; } //once | recurring ( 1 day )

        public OnlyTime OnceAt { get; set; }

        public int Frecuency { get; set; } //just the number

        public DailyFrecuencyType DailyFrecuencyType { get; set; } // Evey "X" Hours, minutes, etc

        public TimeLimits TimeLimits { get; set; }
    }
}
