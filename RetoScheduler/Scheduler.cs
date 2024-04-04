using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using System.Security.Authentication.ExtendedProtection;

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
                throw new SchedulerException("You need to check field to Run Program");
            }
            if (config.Limits == null)
            {
                throw new SchedulerException("Limits Can`t be null");
            }
        }

        private OutPut InOnce(Configuration config)
        {
            if (config.ConfigDateTime.HasValue == false)
            {
                throw new SchedulerException("Once Types requires an obligatory DateTime");
            }

            var dateTime = config.ConfigDateTime.Value;

            if (config.Limits.StartDate > config.Limits.EndDate)
            {
                throw new SchedulerException("The end date cannot be earlier than the initial date");

            }

            if (dateTime < config.Limits.StartDate && dateTime > config.Limits.EndDate)
            {
                throw new SchedulerException("DateTime can't be out of start and end range");
            }

            string expectedDate = dateTime.ToString("dd/MM/yyyy");
            string expectedTime = dateTime.ToString("HH:mm");
            string expectedStartLimit = config.Limits.StartDate.ToString("dd/MM/yyyy");

            var description = "Occurs once. Schedule will be used on " + expectedDate + " at " + expectedTime + " starting on " + expectedStartLimit;

            return new OutPut(dateTime, description);

        }

        private OutPut InRecurring(Configuration config)
        {
            if (config.FrecuencyInDays <= 0)
            {
                throw new SchedulerException("Don't should put negative numbers or zero in number field");
            }

            var dateTime = config.CurrentDate.AddDays(config.FrecuencyInDays);
            if (config.Limits.StartDate > config.Limits.EndDate )
            {
                throw new SchedulerException("The end date cannot be earlier than the initial date");

            }

            //(config.Limits.StartDate < dateTime && config.Limits.EndDate < dateTime) || config.Limits.EndDate < dateTime || (config.Limits.StartDate > dateTime && config.Limits.EndDate > dateTime) || (config.Limits.StartDate < dateTime && config.Limits.EndDate == null)
            var dateBetweenLimits = dateTime > config.Limits.StartDate && (config.Limits.EndDate.HasValue == false || dateTime < config.Limits.EndDate);
            if (dateBetweenLimits ==false)
            {

                throw new SchedulerException("DateTime can't be out of start and end range");

            }





            string expectedStartLimit = config.Limits.StartDate.ToString("dd/MM/yyyy");

            string expectedDate = dateTime.ToString("dd/MM/yyyy");
            var description = "Occurs every day. Schedule will be used on " + expectedDate + " starting on " + expectedStartLimit;

            return new OutPut(dateTime, description);
        }

    }

}
