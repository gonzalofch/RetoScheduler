using RetoScheduler.Enums;

namespace RetoScheduler.Configurations
{
    public class MonthlyConfiguration
    {

        private MonthlyConfiguration(MonthlyConfigType type, int dayNumber, Ordinal ordinalNumber, KindOfDay kindOfDay, int frecuency)
        {
            Type = type;
            DayNumber = dayNumber;
            OrdinalNumber = ordinalNumber;
            SelectedDay = kindOfDay;
            Frecuency = frecuency;
        }

        public static MonthlyConfiguration DayOption(int dayNumber, int frecuency)
        {
            return new MonthlyConfiguration(MonthlyConfigType.DayNumberOption, dayNumber, Ordinal.First, KindOfDay.Day, frecuency);
        }

        public static MonthlyConfiguration WeekDayOption(Ordinal ordinalNumber, KindOfDay kindOfDay, int frecuency)
        {
            return new MonthlyConfiguration(MonthlyConfigType.WeekDayOption, 0, ordinalNumber, kindOfDay, frecuency);
        }

        public MonthlyConfigType Type { get; set; }

        public int DayNumber { get; set; }

        public Ordinal OrdinalNumber { get; set; }

        public KindOfDay SelectedDay { get; set; }

        public int Frecuency { get; set; }
    }
}

