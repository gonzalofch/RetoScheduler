using RetoScheduler.Configurations.Limits;
using RetoScheduler.Enums;

namespace RetoScheduler.Configurations
{
    public class DailyConfiguration
    {
        private DailyConfiguration(DailyConfigType type, TimeOnly onceAt, int frecuency, DailyFrecuency? dailyFrecuencyType, TimeLimits? timeLimits = null)
        {

            Type = type;
            OnceAt = (type == DailyConfigType.Once) ? onceAt : new TimeOnly(0, 0, 0);
            Frecuency = (type == DailyConfigType.Recurring) ? frecuency : null;
            DailyFrecuencyType = (type == DailyConfigType.Recurring) ? dailyFrecuencyType : null;
            TimeLimits = timeLimits;
        }

        public static DailyConfiguration Once(TimeOnly onceAt, TimeLimits? timeLimits = null)
        {
            return new DailyConfiguration(DailyConfigType.Once, onceAt, 0, null, timeLimits);
        }

        public static DailyConfiguration Recurring(int frecuency, DailyFrecuency dailyFrecuencyType, TimeLimits timeLimits)
        {
            return new DailyConfiguration(DailyConfigType.Recurring, TimeOnly.MinValue, frecuency, dailyFrecuencyType, timeLimits);
        }

        public DailyConfigType Type { get; }

        public TimeOnly OnceAt { get; }

        public int? Frecuency { get; }

        public DailyFrecuency? DailyFrecuencyType { get; } // Evey "X" Hours, minutes, etc

        public TimeLimits TimeLimits { get; }
    }
}
