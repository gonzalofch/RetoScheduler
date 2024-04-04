namespace RetoScheduler
{
    public class Scheduler
    {

        public Scheduler()
        {

        }

        public OutPut Execute(Configuration config)
        {
            ValidateConfiguration(config);

            if (config.Type == ConfigType.Once)
            {
                return InOnce(config);
            }
            return InRecurring(config);
        }

        private void ValidateConfiguration(Configuration config)
        {
            if (!config.Enabled)
            {
                throw new Exception("You need to check field to Run Program");
            }
        }

        private OutPut InOnce(Configuration config)
        {
            if (config.ConfigDateTime.HasValue == false)
            {
                throw new Exception("Once Types requires an obligatory DateTime");
            }

            var dateTime = config.ConfigDateTime.Value;
            string expectedDate = dateTime.ToString("dd/MM/yyyy");
            string expectedTime = dateTime.ToString("HH:mm");
            var description = "Occurs once. Schedule will be used on " + expectedDate + " at " + expectedTime + " starting on 01/01/2020";

            return new OutPut(dateTime, description);

        }

        private OutPut InRecurring(Configuration config)
        {
            if (config.FrecuencyInDays <= 0)
            {
                throw new Exception("Don't should put negative numbers or zero in number field");
            }

            var dateTime = config.CurrentDate.AddDays(config.FrecuencyInDays);
            string expectedDate = dateTime.ToString("dd/MM/yyyy");
            var description = "Occurs every day. Schedule will be used on " + expectedDate + " starting on 01/01/2020";

            return new OutPut(dateTime, description);
        }

    }

}
