using Microsoft.Extensions.Localization;
using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using RetoScheduler.Extensions;
using RetoScheduler.Localization;
using RetoScheduler.Runners;
using System.Globalization;

namespace RetoScheduler
{
    public class Scheduler
    {
        private IStringLocalizer L;

        public Scheduler()
        {
            L = new SchedulerLocalizer();
        }

        private bool Executed { get; set; }

        public OutPut Execute(Configuration config)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(config.Cultures.GetDescription());
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(config.Cultures.GetDescription());
            L = new SchedulerLocalizer();
            ValidateConfiguration(config);
            InOnceRunner OnceRunner = new InOnceRunner(L);
            InRecurringRunner RecurringRunner = new InRecurringRunner(L,Executed);
            DateTime nextExecution = config.Type == ConfigType.Once
                ? OnceRunner.Run(config)
                : RecurringRunner.Run(config);

            ValidateNextExecutionIsBetweenDateLimits(config, nextExecution);
            Executed = true;
            DescriptionBuilder descriptionMessageBuilder = new(new SchedulerLocalizer());
            string descriptionMessage = descriptionMessageBuilder.CalculateDescription(nextExecution, config);

            return new OutPut(nextExecution, descriptionMessage);
        }

        private void ValidateConfiguration(Configuration config)
        {
            ValidateSchedulerIsEnabled(config);
            ValidateTimeLimitsAreInRange(config);
            ValidateDateLimitsAreInRange(config);
        }

        private void ValidateSchedulerIsEnabled(Configuration config)
        {
            if (!config.Enabled)
            {
                throw new SchedulerException(L["Scheduler:Errors:NotEnabled"]);
            }
        }

        private void ValidateDateLimitsAreInRange(Configuration config)
        {
            if (config.DateLimits == null)
            {
                throw new SchedulerException(L["Scheduler:Errors:NullDateLimits"]);
            }

            if (config.DateLimits.EndDate < config.DateLimits.StartDate)
            {
                throw new SchedulerException(L["Scheduler:Errors:EndDateEarlierThanStartDate"]);
            }
        }

        private void ValidateTimeLimitsAreInRange(Configuration config)
        {
            bool isRecurring = config.DailyConfiguration.Type == DailyConfigType.Recurring;
            bool hasTimeLimits = config.DailyConfiguration.TimeLimits != null;
            bool endTimeIsShorter = isRecurring && config.DailyConfiguration.TimeLimits.EndTime < config.DailyConfiguration.TimeLimits.StartTime;
            if (hasTimeLimits && endTimeIsShorter)
            {
                throw new SchedulerException(L["Scheduler:Errors:EndTimeEarlierThanStartTime"]);
            }
        }

        private void ValidateNextExecutionIsBetweenDateLimits(Configuration config, DateTime dateTime)
        {
            bool startOutOfLimits = dateTime >= config.DateLimits.StartDate;
            bool endOutOfLimits = config.DateLimits.EndDate.HasValue == false || dateTime <= config.DateLimits.EndDate;
            var dateBetweenLimits = startOutOfLimits && endOutOfLimits;

            if (dateTime < config.CurrentDate)
            {
                throw new SchedulerException(L["Scheduler:Errors:ExecutionEarlierThanCurrentTime"]);
            }

            if (dateBetweenLimits is false)
            {
                throw new SchedulerException(L["Scheduler:Errors:DateOutOfRanges"]);
            }
        }
    }
}