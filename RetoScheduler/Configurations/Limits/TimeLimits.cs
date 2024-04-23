namespace RetoScheduler.Configurations.Limits
{
    public class TimeLimits
    {
        public TimeLimits(TimeOnly startTime, TimeOnly endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }
        public TimeOnly StartTime { get; }

        public TimeOnly EndTime { get; }
    }
}
