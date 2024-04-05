using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using System;
using System.Security.AccessControl;
using System.Security.Authentication.ExtendedProtection;
using System.Text;

namespace RetoScheduler
{
    public class Scheduler
    {

        public Scheduler()
        {
                
        }

        private bool Executed { get; set; }

        public OutPut Execute(Configuration config)
        {
            ValidateConfiguration(config);

            DateTime dateTime;
            if (config.Type == ConfigType.Once)
            {
                dateTime = InOnce(config);
            }
            else
            {
                dateTime = InRecurring(config);
            }
            var description = CalculateDescription(dateTime, config);
            Executed = true;
            return new OutPut(dateTime, description);
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
            ValidateLimitsRange(config);
        }

        private void ValidateLimitsRange(Configuration config)
        {
            if (config.Limits.StartDate > config.Limits.EndDate)
            {
                throw new SchedulerException("The end date cannot be earlier than the initial date");
            }
        }

        private string CalculateDescription(DateTime dateTime, Configuration config)
        {
            StringBuilder description = new StringBuilder("Occurs ");

            string whenOccurs;
            string atThisTime;
            if (config.Type == ConfigType.Once)
            {
                whenOccurs = "once";
                string expectedTime = dateTime.ToString("HH:mm");
                atThisTime = " at " + expectedTime;
            }
            else
            {
                whenOccurs = config.FrecuencyInDays == 1
                ? "every day"
                : "every " + config.FrecuencyInDays + " days";
                atThisTime = string.Empty;
            }

            string expectedDate= dateTime.ToString("dd/MM/yyyy");
            string expectedStartLimit = config.Limits.StartDate.ToString("dd/MM/yyyy");

            description.Append(whenOccurs)
                .Append(". Schedule will be used on ")
                .Append(expectedDate)
                .Append(atThisTime)
                .Append(" starting on ")
                .Append(expectedStartLimit);

            return description.ToString();
        }

        private DateTime InOnce(Configuration config)
        {
            if (config.ConfigDateTime.HasValue == false)
            {
                throw new SchedulerException("Once Types requires an obligatory DateTime");
            }

            var dateTime = config.ConfigDateTime.Value;

            return dateTime;

        }

        private DateTime InRecurring(Configuration config)
        {
            if (config.FrecuencyInDays <= 0)
            {
                throw new SchedulerException("Don't should put negative numbers or zero in number field");
            }
            var dateTime = config.CurrentDate < config.Limits.StartDate
                ? config.Limits.StartDate
                : config.CurrentDate;

            if (Executed)
            {
                dateTime = dateTime.AddDays(config.FrecuencyInDays);
            }

            var dateBetweenLimits = dateTime >= config.Limits.StartDate && (config.Limits.EndDate.HasValue == false || dateTime <= config.Limits.EndDate);
            if (dateBetweenLimits == false)
            {
                throw new SchedulerException("DateTime can't be out of start and end range");
            }

            return dateTime;
        }
    }
}
