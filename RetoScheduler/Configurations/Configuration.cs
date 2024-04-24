using RetoScheduler.Configurations.Limits;
using RetoScheduler.Enums;
using System.Globalization;

namespace RetoScheduler.Configurations
{
    public class Configuration
    {
        public Configuration(DateTime currentDate, ConfigType type, bool enabled, DateTime? configDataTime, Occurs occurs,MonthlyConfiguration? monthlyConfiguration, WeeklyConfiguration? weeklyConfiguration, DailyConfiguration dailyConfiguration, DateLimits dateLimits, Cultures cultures = Enums.Cultures.en_US)
        {
            CurrentDate = currentDate;
            Type = type;
            Enabled = enabled;
            ConfigDateTime = (type == ConfigType.Once) ? configDataTime : null;
            Occurs = occurs;
            MonthlyConfiguration = monthlyConfiguration;
            WeeklyConfiguration = weeklyConfiguration;
            DailyConfiguration = dailyConfiguration;
            DateLimits = dateLimits;
            Cultures = cultures;
        }

        public DateTime CurrentDate { get; }

        public ConfigType Type { get; }

        public bool Enabled { get; }

        public DateTime? ConfigDateTime { get; }

        public Occurs Occurs { get; }

        public MonthlyConfiguration? MonthlyConfiguration { get; }

        public WeeklyConfiguration? WeeklyConfiguration { get; }

        public DailyConfiguration DailyConfiguration { get; }

        public DateLimits DateLimits { get; }

        public Cultures? Cultures { get;}
    }
}
