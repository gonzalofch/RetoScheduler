namespace RetoScheduler.Configurations
{
    public class WeeklyConfiguration
    {
        public WeeklyConfiguration(int frecuencyInWeeks, List<DayOfWeek> selectedDays)
        {
            FrecuencyInWeeks = frecuencyInWeeks;
            SelectedDays = selectedDays;
        }

        public int FrecuencyInWeeks { get; }

        public List<DayOfWeek> SelectedDays { get; }
    }
}
