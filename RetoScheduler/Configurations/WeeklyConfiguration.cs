namespace RetoScheduler.Configurations
{
    public class WeeklyConfiguration
    {
        public WeeklyConfiguration(int frecuencyInWeeks, List<DayOfWeek> selectedDays)
        {
            FrecuencyInWeeks = frecuencyInWeeks;
            SelectedDays = selectedDays;
        }

        public int FrecuencyInWeeks { get; set; }

        public List<DayOfWeek> SelectedDays { get; set; }
    }
}
