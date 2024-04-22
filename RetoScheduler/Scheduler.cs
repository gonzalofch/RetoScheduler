﻿using Microsoft.VisualBasic;
using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using RetoScheduler.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            if (config.DailyConfiguration.Type == DailyConfigType.Recurring && config.DailyConfiguration.TimeLimits != null)
            {
                if (config.DailyConfiguration.TimeLimits.EndTime < config.DailyConfiguration.TimeLimits.StartTime)
                {
                    throw new SchedulerException("The EndTime cannot be earlier than StartTime");
                }
            }
        }

        private string CalculateDescription(DateTime dateTime, Configuration config)
        {
            StringBuilder description = new StringBuilder("Occurs ");

            if (config.Type == ConfigType.Once && config.ConfigDateTime.HasValue)
            {
                description.Append("once at " + dateTime.ToString("dd/MM/yyyy "));
            }
            else
            {
                description.Append(GetWeeklyDescription(config));
            }

            if (config.DailyConfiguration.Type == DailyConfigType.Once && config.DailyConfiguration.OnceAt != new TimeOnly(0, 0, 0))
            {
                string tiempoDeEjecucion = config.DailyConfiguration.OnceAt.ParseAmPm();
                description.Append("one time at " + tiempoDeEjecucion + " ");
            }
            else
            {
                description.Append(GetDailyDescription(config));
                description.Append(GetDateLimitsDescription(config));
            }


            return description.ToString();
        }

        private static string GetDateLimitsDescription(Configuration config)
        {
            string startDate = config.DateLimits.StartDate.Date.ToString("dd/MM/yyyy");
            string endDate = config.DateLimits.EndDate?.ToString("dd/MM/yyyy");
            return config.DateLimits.EndDate.HasValue
                ? "starting on " + startDate + " and finishing on " + endDate
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
            if (config.WeeklyConfiguration == null)
            {
                return string.Empty;
            }
            string weeklyMessage = config.WeeklyConfiguration.FrecuencyInWeeks switch
            {
                0 => "week ",
                1 => config.WeeklyConfiguration.FrecuencyInWeeks + " week ",
                > 1 => config.WeeklyConfiguration.FrecuencyInWeeks + " weeks ",
                _ => throw new SchedulerException("Not supported action for weekly frecuency message"),
            };
            return weeklyMessage + GetStringListDayOfWeek(config.WeeklyConfiguration.SelectedDays);
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
            string timeUnit = config.DailyConfiguration.DailyFrecuencyType switch
            {
                DailyFrecuency.Hours => "hours",
                DailyFrecuency.Minutes => "minutes",
                DailyFrecuency.Seconds => "seconds",
                _ => throw new SchedulerException("Not supported action for daily frecuency message"),
            };

            return "every " + config.DailyConfiguration.Frecuency + " " + timeUnit + " between ";
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

            var dateTime = GetFirstExecutionDate(config);
            dateTime = NextExecutionDate(config, dateTime);

            return NextExecutionTime(config.DailyConfiguration, dateTime);
        }

        private DateTime NextExecutionTime(DailyConfiguration dailyConfiguration, DateTime dateTime)
        {
            if (dailyConfiguration.Type == DailyConfigType.Once)
            {
                return dateTime.Date.Add(dailyConfiguration.OnceAt.ToTimeSpan());
            }

            TimeOnly nextExecutionTime = TimeOnly.FromDateTime(dateTime);
            if (Executed)
            {
                nextExecutionTime = AddOccursEveryUnit(dailyConfiguration, nextExecutionTime);
            }

            return nextExecutionTime < dailyConfiguration.TimeLimits.StartTime
                ? AddStartTime(dailyConfiguration, dateTime)
                : AddNextExecutionTime(dateTime, nextExecutionTime);
        }

        private static DateTime AddNextExecutionTime(DateTime dateTime, TimeOnly nextExecutionTime)
        {
            return dateTime.Date + nextExecutionTime.ToTimeSpan();
        }

        private static DateTime AddStartTime(DailyConfiguration dailyConfiguration, DateTime dateTime)
        {
            return dateTime.Date + dailyConfiguration.TimeLimits.StartTime.ToTimeSpan();
        }

        private DateTime GetFirstExecutionDate(Configuration config)
        {
            if (Executed)
            {
                return config.CurrentDate;
            }

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
        }

        private DateTime NextExecutionDate(Configuration config, DateTime dateTime)
        {
            if (config.MonthlyConfiguration != null)
            {
                return GetNextMonthDate(config.MonthlyConfiguration, config.DailyConfiguration, dateTime);
            }

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

        public DateTime GetNextMonthDate(MonthlyConfiguration monthlyConfiguration, DailyConfiguration dailyConfiguration, DateTime dateTime)
        {
            DateTime nextMonthDate = dateTime;
            if (monthlyConfiguration.Type == MonthlyConfigType.DayNumberOption)
            {
                return NextDayInMonth(nextMonthDate, monthlyConfiguration, dailyConfiguration);
            }
            else
            {
                return Executed
                    ? ExecutedNextDayOfWeekInMonth(monthlyConfiguration, dailyConfiguration, dateTime)
                    : NextDayOfWeekInMonth(monthlyConfiguration, dateTime);
            }
        }

        private DateTime ExecutedNextDayOfWeekInMonth(MonthlyConfiguration monthlyConfiguration, DailyConfiguration dailyConfiguration, DateTime dateTime)
        {
            if (NextExecutionTime(dailyConfiguration, dateTime).TimeOfDay <= dailyConfiguration.TimeLimits.EndTime.ToTimeSpan())
            {
                return dateTime;
            }
            else
            {
                dateTime = new DateTime(dateTime.AddMonths(monthlyConfiguration.Frecuency).Year, dateTime.AddMonths(monthlyConfiguration.Frecuency).Month, 1);

                return NextDayOfWeekInMonth(monthlyConfiguration, dateTime);
            }
        }

        private static DateTime NextDayOfWeekInMonth(MonthlyConfiguration monthlyConfig, DateTime currentDate)
        {
            Month month = new Month(currentDate.Year, currentDate.Month);

            List<DayOfWeek> selectedDays = monthlyConfig.SelectedDay switch
            {
                KindOfDay.Day => new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday },
                KindOfDay.WeekDay => new List<DayOfWeek>() { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday },
                KindOfDay.WeekEndDay => new List<DayOfWeek>() { DayOfWeek.Saturday, DayOfWeek.Sunday },
                KindOfDay.Monday => new List<DayOfWeek>() { DayOfWeek.Monday },
                KindOfDay.Tuesday => new List<DayOfWeek>() { DayOfWeek.Tuesday },
                KindOfDay.Wednesday => new List<DayOfWeek>() { DayOfWeek.Wednesday },
                KindOfDay.Thursday => new List<DayOfWeek>() { DayOfWeek.Thursday },
                KindOfDay.Friday => new List<DayOfWeek>() { DayOfWeek.Friday },
                KindOfDay.Saturday => new List<DayOfWeek>() { DayOfWeek.Saturday },
                KindOfDay.Sunday => new List<DayOfWeek>() { DayOfWeek.Sunday },
                _ => throw new SchedulerException("The selected Kind of Day is not supported"),
            };

            IReadOnlyList<DateTime> listOfDays;
            if (selectedDays.Count != 1)
            {
                listOfDays = month.GetMonthDays()
                   .Where(x => x >= currentDate)
                   .Select(x => x.Add(TimeOnly.FromDateTime(currentDate).ToTimeSpan()))
                   .ToList();

                return ObtainOrdinalsFromList(listOfDays.Where(x => selectedDays.Contains(x.DayOfWeek)).ToList(), monthlyConfig);

            }
            listOfDays = month.GetMonthDays(selectedDays.First())
                    .Where(x => x >= currentDate)
                    .Select(x => x.Add(TimeOnly.FromDateTime(currentDate).ToTimeSpan()))
                    .ToList();

            return ObtainOrdinalsFromList(listOfDays, monthlyConfig);
        }

        private static DateTime ObtainOrdinalsFromList(IReadOnlyList<DateTime> list, MonthlyConfiguration monthlyConfig)
        {
            bool greaterThanIndex1 = (Ordinal.First == monthlyConfig.OrdinalNumber || Ordinal.Last == monthlyConfig.OrdinalNumber) && list.Count < 1;
            bool greaterThanIndex2 = Ordinal.Second == monthlyConfig.OrdinalNumber && list.Count < 2;
            bool greaterThanIndex3 = Ordinal.Third == monthlyConfig.OrdinalNumber && list.Count < 3;
            bool greaterThanIndex4 = Ordinal.Fourth == monthlyConfig.OrdinalNumber && list.Count < 4;
            if (greaterThanIndex1 || greaterThanIndex2 || greaterThanIndex3 || greaterThanIndex4)
            {
                throw new SchedulerException("The index is greater than the number of days");
            }
            var dateTime = monthlyConfig.OrdinalNumber switch
            {
                Ordinal.First => list[0],
                Ordinal.Second => list[1],
                Ordinal.Third => list[2],
                Ordinal.Fourth => list[3],
                Ordinal.Last => list.Last(),
                _ => DateTime.MinValue,
            };

            return dateTime;
        }

        public DateTime NextDayInMonth(DateTime dateTime, MonthlyConfiguration monthlyConfig, DailyConfiguration dailyConfiguration)
        {
            int dayNumber = monthlyConfig.DayNumber;
            bool exceedsTheDateTime = !(dailyConfiguration.Type == DailyConfigType.Recurring
                && dateTime <= new DateTime(dateTime.Year, dateTime.Month, dayNumber, dateTime.Hour, dateTime.Minute, dateTime.Second)
                && NextExecutionTime(dailyConfiguration, dateTime).TimeOfDay <= dailyConfiguration.TimeLimits.EndTime.ToTimeSpan());

            if (exceedsTheDateTime)
            {
                dateTime = Executed
                ? dateTime.AddMonths(monthlyConfig.Frecuency + 1)
                : dateTime.AddDays(1).Date;
            }

            return dateTime.Day <= dayNumber && dayNumber <= DateTime.DaysInMonth(dateTime.Year, dateTime.Month)
                ? ExcedsDays(dateTime, dayNumber, exceedsTheDateTime)
                : GetNextPossibleDate(dateTime, dayNumber);
        }

        private static DateTime ExcedsDays(DateTime dateTime, int dayNumber, bool exceedsTheDateTime)
        {
            return exceedsTheDateTime
                                ? new DateTime(dateTime.Year, dateTime.Month, dayNumber)
                                : new DateTime(dateTime.Year, dateTime.Month, dayNumber, dateTime.Hour, dateTime.Minute, dateTime.Second);
        }

        private static DateTime GetNextPossibleDate(DateTime dateTime, int dayNumber)
        {
            dateTime = dateTime.AddMonths(1);
            while (DateTime.DaysInMonth(dateTime.Year, dateTime.Month) < dayNumber)
            {
                dateTime = dateTime.AddMonths(1);
            }

            return new DateTime(dateTime.Year, dateTime.Month, dayNumber);
        }

        private static TimeOnly AddOccursEveryUnit(DailyConfiguration dailyConfiguration, TimeOnly dateTimeTime)
        {
            if (dailyConfiguration.Frecuency == null)
            {
                return dateTimeTime;
            }

            if (dateTimeTime == TimeOnly.MinValue)
            {
                return dailyConfiguration.TimeLimits.StartTime;
            }

            return dailyConfiguration.DailyFrecuencyType switch
            {
                DailyFrecuency.Hours => dateTimeTime.AddHours(dailyConfiguration.Frecuency.Value),
                DailyFrecuency.Minutes => dateTimeTime.AddMinutes(dailyConfiguration.Frecuency.Value),
                DailyFrecuency.Seconds => dateTimeTime.AddSeconds(dailyConfiguration.Frecuency.Value),
                _ => dateTimeTime,
            };
        }
    }
}