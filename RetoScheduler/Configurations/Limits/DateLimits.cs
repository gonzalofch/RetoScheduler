namespace RetoScheduler.Configurations
{
    public class DateLimits
    {

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public DateLimits(DateTime startDate, DateTime? endDate = null)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
