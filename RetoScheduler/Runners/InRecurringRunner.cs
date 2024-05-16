using Microsoft.Extensions.Localization;
using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using RetoScheduler.Extensions;

namespace RetoScheduler.Runners
{
    public class InRecurringRunner
    {
        private bool Executed = false;

        public InRecurringRunner(bool executed)
        {
            Executed = executed;
        }

        public DateTime Run(Configuration config)
        {
            if (config.DailyConfiguration.Frecuency <= 0)
            {
                throw new SchedulerException("Scheduler:Errors:NegativeDailyFrecuency");
            }
            DateTime firstExecution = Executed
                ? config.CurrentDate
                : GetFirstExecutionDate(config);


            if (NextExecutionDate(config, firstExecution) < config.DateLimits.StartDate)
            {
                int diffMonths = ((config.DateLimits.StartDate.Year - config.CurrentDate.Year) * 12) + config.DateLimits.StartDate.Month + 1 - config.CurrentDate.Month;
                DateTime possibleStartDate = config.CurrentDate.AddMonths(diffMonths).JumpToDayNumber(config.DateLimits.StartDate.Day).Date;
                return NextExecutionDate(config, possibleStartDate);
            }
            return NextExecutionDate(config, firstExecution);

        }

        private DateTime GetFirstExecutionDate(Configuration config)
        {
            return config.CurrentDate < config.DateLimits.StartDate
                ? config.DateLimits.StartDate
                : config.CurrentDate;
        }

        private DateTime NextExecutionDate(Configuration config, DateTime dateTime)
        {
       
            bool hasLimits = config.DailyConfiguration.TimeLimits != null;

            if (config.MonthlyConfiguration != null)
            {
                var monthlyDate = MonthlyRunner.Run(config.MonthlyConfiguration, dateTime, Executed);
                var dailyDate = DailyRunner.Run(config.DailyConfiguration, monthlyDate, Executed);

                return dailyDate;
            }

            if (config.WeeklyConfiguration == null || !config.WeeklyConfiguration.SelectedDays.Any())
            {
                return DailyRunner.Run(config.DailyConfiguration, dateTime, Executed);
            }

            List<DayOfWeek> sortedDays = config.WeeklyConfiguration.SelectedDays.OrderBy(_ => _).ToList();
            DayOfWeek actualDayOfWeek = config.CurrentDate.DayOfWeek;
            bool excedsLimits = hasLimits && config.CurrentDate.TimeOfDay >= config.DailyConfiguration.TimeLimits.EndTime.ToTimeSpan();
            bool skipDay = excedsLimits || (config.DailyConfiguration.Type == DailyConfigType.Once && Executed);

            return skipDay
                ? DailyRunner.Run(config.DailyConfiguration, GetNextDayInWeek(sortedDays, actualDayOfWeek, dateTime, config), Executed)
            
                : DailyRunner.Run(config.DailyConfiguration, NextDay(sortedDays, actualDayOfWeek, dateTime), Executed) ;
        }

        private static DateTime NextDay(List<DayOfWeek> sortedDays, DayOfWeek actualDay, DateTime dateTime)
        {
            DayOfWeek nextDay = sortedDays.FirstOrDefault(_ => _ >= actualDay);

            return dateTime.NextDayOfWeek(nextDay);
        }

        private static DateTime GetNextDayInWeek(List<DayOfWeek> sortedDays, DayOfWeek actualDay, DateTime dateTime, Configuration config)
        {
            DayOfWeek nextDay = sortedDays.FirstOrDefault(_ => _ > actualDay);

            if (!sortedDays.Contains(nextDay))
            {
                nextDay = sortedDays.FirstOrDefault(_ => _ > dateTime.NextDayOfWeek(nextDay).DayOfWeek);
                dateTime = dateTime.AddWeeks(config.WeeklyConfiguration.FrecuencyInWeeks);
            }

            return dateTime.NextDayOfWeek(nextDay).Date;
        }
    }
}
