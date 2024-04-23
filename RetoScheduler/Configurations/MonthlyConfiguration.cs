using RetoScheduler.Enums;

namespace RetoScheduler.Configurations
{
    public class MonthlyConfiguration
    {

        public MonthlyConfiguration(MonthlyConfigType type, int dayNumber, Ordinal ordinalNumber, KindOfDay kindOfDay, int frecuency)
        {
            Type = type;
            DayNumber =  dayNumber;
            OrdinalNumber = ordinalNumber ;
            SelectedDay =   kindOfDay ;
            Frecuency = frecuency;
        }

        public MonthlyConfigType Type { get; set; }

        public int DayNumber { get; set; }

        public Ordinal OrdinalNumber { get; set; }

        public KindOfDay SelectedDay { get; set; }

        public int Frecuency { get; set; }
    }
}

