namespace RetoScheduler
{
    public class OutPut
    {
        public DateTime NextExecutionTime { get; }

        public string Description { get; }

        public OutPut(DateTime nextExecutionTime, string description)
        {
            NextExecutionTime = nextExecutionTime;
            Description = description;
        }
    }
}
