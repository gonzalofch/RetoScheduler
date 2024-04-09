using RetoScheduler.Configurations.Limits;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;

namespace RetoScheduler.Configurations
{
    public class Configuration
    {
        public Configuration(DateTime currentDate, ConfigType type, bool enabled, DateTime? configDataTime, Occurs occurs, WeeklyConfiguration weeklyConfiguration, DailyConfiguration dailyConfiguration, DateLimits dateLimits) //bool tal vez por defecto true
        {
            CurrentDate = currentDate;
            Enabled = enabled;
            ConfigDateTime = configDataTime;
            Type = type;
            Occurs = occurs;
            WeeklyConfiguration = weeklyConfiguration;
            DailyConfiguration = dailyConfiguration;
            DateLimits = dateLimits;
        }

        public DateTime CurrentDate { get; }

        public ConfigType Type { get; }

        public bool Enabled { get; }

        public DateTime? ConfigDateTime { get; }

        public Occurs Occurs { get; }

        public WeeklyConfiguration WeeklyConfiguration { get; set; }

        public DailyConfiguration DailyConfiguration { get; set; }

        public DateLimits DateLimits { get; }

        
    }
}
