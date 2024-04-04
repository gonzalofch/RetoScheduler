namespace RetoScheduler
{
    public class Configuration
    {
        public Configuration(DateTime currentDate, ConfigType type, bool enabled, DateTime? configDataTime, Occurs occurs, int frecuencyInDays) //bool tal vez por defecto true
        {
            CurrentDate = currentDate;
            Enabled = enabled;
            ConfigDateTime = configDataTime;
            Type = type;
            Occurs = occurs;
            FrecuencyInDays = frecuencyInDays;
        }

        public DateTime CurrentDate { get; }

        public ConfigType Type { get; }

        public bool Enabled { get; }

        public DateTime? ConfigDateTime { get; }

        public Occurs Occurs { get; }

        public int FrecuencyInDays { get; }
    }
}
