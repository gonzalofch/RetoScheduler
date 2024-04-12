using Microsoft.VisualBasic;
using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using RetoScheduler.Extensions;
using System;
using System.Linq;
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

            DateTime dateTime = config.Type == ConfigType.Once
                ? InOnce(config)
                : InRecurring(config);

            ValidateNextExecutionIsBetweenDateLimits(config, dateTime);
            Executed = true;

            return new OutPut(dateTime, CalculateDescription(dateTime, config));
        }

        private void ValidateConfiguration(Configuration config)
        {
            ValidadIsEnabled(config);
            ValidateLimitsRange(config);
            ValidateLimitsTimeRange(config);
        }

        private static void ValidadIsEnabled(Configuration config)
        {
            if (!config.Enabled)
            {
                throw new SchedulerException("You need to check field to Run Program");
            }
        }

        private void ValidateLimitsRange(Configuration config)
        {
            if (config.DateLimits == null)
            {
                throw new SchedulerException("Limits Can`t be null");
            }
            if (config.DateLimits.StartDate > config.DateLimits.EndDate)
            {
                throw new SchedulerException("The end date cannot be earlier than the initial date");
            }
        }

        private void ValidateLimitsTimeRange(Configuration config)
        {
            if (config.DailyConfiguration.Type == DailyConfigType.Recurring)
            {
                if (config.DailyConfiguration.TimeLimits.EndTime.ToTimeSpan() < config.CurrentDate.TimeOfDay)
                {
                    throw new SchedulerException("The EndTime cannot be earlier than Current Time");
                }

                if (config.DailyConfiguration.TimeLimits.EndTime < config.DailyConfiguration.TimeLimits.StartTime)
                {
                    throw new SchedulerException("The EndTime cannot be earlier than StartTime");
                }
            }
        }

        private string CalculateDescription(DateTime dateTime, Configuration config)
        {
            StringBuilder description = new StringBuilder("Occurs ");

            if (config.Type == ConfigType.Once)
            {
                description.Append("once at " + config.ConfigDateTime.Value.ToString("dd/MM/yyyy "));
            }
            else
            {
                description.Append(GetWeeklyDescription(config));
            }

            if (config.DailyConfiguration.Type == DailyConfigType.Once)
            {
                string tiempoDeEjecucion = config.DailyConfiguration.OnceAt.ParseAmPm();
                description.Append("one time at " + tiempoDeEjecucion + " ");
            }
            else
            {
                description.Append(GetDailyDescription(config));
            }

            description.Append(GetDateLimitsDescription(config));

            return description.ToString();
        }

        private static string GetDateLimitsDescription(Configuration config)
        {
            string startDate = config.DateLimits.StartDate.Date.ToString("dd/MM/yyyy");
            string endDate = config.DateLimits.EndDate?.ToString("dd/MM/yyyy");
            return config.DateLimits.EndDate.HasValue
                ? "starting on " + startDate + " and finishing on "+endDate
                : "starting on " + startDate;
        }

        private static string GetWeeklyDescription(Configuration config)
        {
            return config.WeeklyConfiguration != null
                ? "every " + GetWeeklyFrecuencyMessage(config)
                : "every day ";
        }

        private static string GetWeeklyFrecuencyMessage(Configuration config)
        {
            return config.WeeklyConfiguration.FrecuencyInWeeks switch
            {
                0 => "week ",
                1 => config.WeeklyConfiguration.FrecuencyInWeeks + " week ",
                > 1 => config.WeeklyConfiguration.FrecuencyInWeeks + " weeks ",
                _ => throw new SchedulerException("Not supported action for weekly frecuency message"),
            } + GetStringListDayOfWeek(config.WeeklyConfiguration.SelectedDays);
        }

        private static string GetDailyDescription(Configuration config)
        {
            string dailyDescription = config.WeeklyConfiguration == null ? "and " : string.Empty;
            string timeStartLimit = config.DailyConfiguration.TimeLimits.StartTime.ParseAmPm();
            string timeEndLimit = config.DailyConfiguration.TimeLimits.EndTime.ParseAmPm();

            return dailyDescription + GetDailyFrecuencyMessage(config) + timeStartLimit + " and " + timeEndLimit + " ";
        }

        private static string GetDailyFrecuencyMessage(Configuration config)
        {

            return "every " + config.DailyConfiguration.Frecuency + " " +
                config.DailyConfiguration.DailyFrecuencyType switch
                {
                    DailyFrecuency.Hours => "hours",
                    DailyFrecuency.Minutes => "minutes",
                    DailyFrecuency.Seconds => "seconds",
                    _ => throw new SchedulerException("Not supported action for daily frecuency message"),
                } + " between ";
        }

        private static string GetStringListDayOfWeek(List<DayOfWeek> selectedDays)
        {
            string formattedList = "on";

            foreach (var item in selectedDays)
            {
                string dayInLower = item.ToString().ToLower();
                if (item == selectedDays.Last() && selectedDays.Count() >= 2)
                {
                    formattedList += " and " + dayInLower + " ";
                }
                else
                {
                    formattedList = item == selectedDays.First()
                        ? formattedList += " "
                        : formattedList += ", ";

                    formattedList += dayInLower;
                }
            }

            return formattedList;
        }

        private DateTime InOnce(Configuration config)
        {
            if (config.ConfigDateTime.HasValue == false)
            {
                throw new SchedulerException("Once Types requires an obligatory DateTime");
            }

            return config.DailyConfiguration.Type == DailyConfigType.Once
                ? config.ConfigDateTime.Value + config.DailyConfiguration.OnceAt.ToTimeSpan()
                : config.ConfigDateTime.Value + config.DailyConfiguration.TimeLimits.StartTime.ToTimeSpan();
        }

        private DateTime InRecurring(Configuration config)
        {
            if (config.DailyConfiguration.Frecuency <= 0)
            {
                throw new SchedulerException("Don't should put negative numbers or zero in number field");
            }

            var dateTime = Executed
                ? config.CurrentDate
                : GetFirstExecutionDate(config);

            dateTime = NextDayExecution(config, dateTime);

            TimeOnly dateTimeTime = TimeOnly.FromDateTime(dateTime);
            if (config.DailyConfiguration.Type == DailyConfigType.Once)
            {
                return dateTime.Date.Add(config.DailyConfiguration.OnceAt.ToTimeSpan());
            }

            TimeOnly nextExecutionTime = AddOccursEveryUnit(config, dateTimeTime);

            return nextExecutionTime < config.DailyConfiguration.TimeLimits.StartTime
                ? dateTime.Date + config.DailyConfiguration.TimeLimits.StartTime.ToTimeSpan()
                : dateTime.Date + nextExecutionTime.ToTimeSpan();
        }
        private DateTime GetFirstExecutionDate(Configuration config)
        {

            return config.CurrentDate < config.DateLimits.StartDate
           ? config.DateLimits.StartDate
           : config.CurrentDate;
        }

        private static void ValidateNextExecutionIsBetweenDateLimits(Configuration config, DateTime dateTime)
        {
            var dateBetweenLimits = dateTime >= config.DateLimits.StartDate && (config.DateLimits.EndDate.HasValue == false || dateTime <= config.DateLimits.EndDate);
            if (dateBetweenLimits is false)
            {
                throw new SchedulerException("DateTime can't be out of start and end range");
            }
            if (dateTime < config.CurrentDate)
            {
                throw new SchedulerException("Execution Time can't be earlier than Current Date");
            }
        }

        private DateTime NextDayExecution(Configuration config, DateTime dateTime)
        {
            if (config.WeeklyConfiguration == null || !config.WeeklyConfiguration.SelectedDays.Any())
            {
                return dateTime;
            }

            var sortedDays = config.WeeklyConfiguration.SelectedDays.OrderBy(_ => _).ToList();
            var actualDay = config.CurrentDate.DayOfWeek;
            var hasLimits = config.DailyConfiguration.TimeLimits != null;
            var excedsLimits = hasLimits && config.CurrentDate.TimeOfDay >= config.DailyConfiguration.TimeLimits.EndTime.ToTimeSpan();
            var skipDay = excedsLimits || (config.DailyConfiguration.Type == DailyConfigType.Once && Executed);

            var firstDayOfWeek = skipDay
                ? sortedDays.FirstOrDefault(_ => _ > actualDay)
                : sortedDays.FirstOrDefault(_ => _ >= actualDay);

            return skipDay
                ? GetNextDayInWeek(sortedDays, actualDay, dateTime, config)
                : NextDay(sortedDays, actualDay, dateTime);
        }

        private static DateTime NextDay(List<DayOfWeek> sortedDays, DayOfWeek actualDay, DateTime dateTime)
        {
            var nextDay = sortedDays.FirstOrDefault(_ => _ >= actualDay);

            return dateTime.NextDayOfWeek(nextDay);
        }

        private static DateTime GetNextDayInWeek(List<DayOfWeek> sortedDays, DayOfWeek actualDay, DateTime dateTime, Configuration config)
        {
            var nextDay = sortedDays.FirstOrDefault(_ => _ > actualDay);
            if (!sortedDays.Contains(nextDay))
            {
                nextDay = sortedDays.FirstOrDefault(_ => _ > dateTime.NextDayOfWeek(nextDay).DayOfWeek);
                dateTime = dateTime.AddWeeks(config.WeeklyConfiguration.FrecuencyInWeeks);
            }

            return dateTime.NextDayOfWeek(nextDay).Date;
        }

        private static TimeOnly AddOccursEveryUnit(Configuration config, TimeOnly dateTimeTime)
        {
            if (dateTimeTime == TimeOnly.MinValue)
            {
                return config.DailyConfiguration.TimeLimits.StartTime;
            }

            return config.DailyConfiguration.DailyFrecuencyType switch
            {
                DailyFrecuency.Hours => dateTimeTime.AddHours(config.DailyConfiguration.Frecuency.Value),
                DailyFrecuency.Minutes => dateTimeTime.AddMinutes(config.DailyConfiguration.Frecuency.Value),
                DailyFrecuency.Seconds => dateTimeTime.AddSeconds(config.DailyConfiguration.Frecuency.Value),
                _ => dateTimeTime,
            };
        }
    }
}