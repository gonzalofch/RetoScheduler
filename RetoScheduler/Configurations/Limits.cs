namespace RetoScheduler.Configurations
{
    public class Limits
    {

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Limits(DateTime startDate, DateTime? endDate = null)
        {
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
