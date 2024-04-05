using RetoScheduler.Enums;
using RetoScheduler.Exceptions;

namespace RetoScheduler.Configurations
{
    public class Configuration
    {
        public Configuration(DateTime currentDate, ConfigType type, bool enabled, DateTime? configDataTime, Occurs occurs, int frecuencyInDays, Limits limits) //bool tal vez por defecto true
        {
            CurrentDate = currentDate;
            Enabled = enabled;
            ConfigDateTime = configDataTime;
            Type = type;
            Occurs = occurs;
            FrecuencyInDays = frecuencyInDays;
            Limits = limits;
        }

        public DateTime CurrentDate { get; }

        public ConfigType Type { get; }

        public bool Enabled { get; }

        public DateTime? ConfigDateTime { get; }

        public Occurs Occurs { get; }

        public int FrecuencyInDays { get; }
        public Limits Limits { get; }
    }
}
