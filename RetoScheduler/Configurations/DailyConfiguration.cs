using RetoScheduler.Configurations.Limits;
using RetoScheduler.Enums;

namespace RetoScheduler.Configurations
{
    public class DailyConfiguration
    {
        public DailyConfiguration(DailyConfigType type, TimeOnly onceAt, int? frecuency, DailyFrecuency? dailyFrecuencyType, TimeLimits? timeLimits=null)
        {

            Type = type;
            OnceAt = (type == DailyConfigType.Once) ? onceAt : new TimeOnly(0,0,0);
            Frecuency = (type == DailyConfigType.Recurring) ? frecuency : null;
            DailyFrecuencyType = (type == DailyConfigType.Recurring) ? dailyFrecuencyType : null;
            TimeLimits = timeLimits;
        }

        public DailyConfigType Type { get; } //once | recurring ( 1 day )

        public TimeOnly OnceAt { get; }

        public int? Frecuency { get; } //just the number

        public DailyFrecuency? DailyFrecuencyType { get; } // Evey "X" Hours, minutes, etc

        public TimeLimits TimeLimits { get; }
    }
}
