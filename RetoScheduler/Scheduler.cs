﻿using Microsoft.Extensions.Localization;
using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using RetoScheduler.Extensions;
using RetoScheduler.Localization;
using System.Globalization;

namespace RetoScheduler
{
    public class Scheduler
    {
        private IStringLocalizer _stringLocalizer;

        public Scheduler()
        {
            _stringLocalizer = new SchedulerLocalizer();
        }

        private bool Executed { get; set; }

        public OutPut Execute(Configuration config)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo(config.Cultures.GetDescription());
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(config.Cultures.GetDescription());
            _stringLocalizer = new SchedulerLocalizer();
            ValidateConfiguration(config);

            DateTime nextExecution = config.Type == ConfigType.Once
                ? InOnce(config)
                : InRecurring(config);

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
                throw new SchedulerException(_stringLocalizer["Scheduler:Errors:NotEnabled"]);
            }
        }

        private void ValidateDateLimitsAreInRange(Configuration config)
        {
            if (config.DateLimits == null)
            {
                throw new SchedulerException(_stringLocalizer["Scheduler:Errors:NullDateLimits"]);
            }

            if (config.DateLimits.EndDate < config.DateLimits.StartDate)
            {
                throw new SchedulerException(_stringLocalizer["Scheduler:Errors:EndDateEarlierThanStartDate"]);
            }
        }

        private void ValidateTimeLimitsAreInRange(Configuration config)
        {
            bool isRecurring = config.DailyConfiguration.Type == DailyConfigType.Recurring;
            bool hasTimeLimits = config.DailyConfiguration.TimeLimits != null;
            bool endTimeIsShorter = isRecurring && config.DailyConfiguration.TimeLimits.EndTime < config.DailyConfiguration.TimeLimits.StartTime;
            if (hasTimeLimits && endTimeIsShorter)
            {
                throw new SchedulerException(_stringLocalizer["Scheduler:Errors:EndTimeEarlierThanStartTime"]);
            }
        }

        private DateTime InOnce(Configuration config)
        {
            if (config.ConfigDateTime.HasValue == false)
            {
                throw new SchedulerException(_stringLocalizer["Scheduler:Errors:RequiredConfigDateTimeInOnce"]);
            }
            DateTime configDateTime = config.ConfigDateTime.Value;

            return config.DailyConfiguration.Type == DailyConfigType.Once
                ? configDateTime.Add(config.DailyConfiguration.OnceAt.ToTimeSpan())
                : configDateTime.Add(config.DailyConfiguration.TimeLimits.StartTime.ToTimeSpan());
        }

        private DateTime InRecurring(Configuration config)
        {
            if (config.DailyConfiguration.Frecuency <= 0)
            {
                throw new SchedulerException(_stringLocalizer["Scheduler:Errors:NegativeDailyFrecuency"]);
            }

            var firstExecution = GetFirstExecutionDate(config);
            var nextExecution = NextExecutionDate(config, firstExecution);
            bool hasJumpedDays = config.CurrentDate.Date != nextExecution.Date;

            return NextExecutionTime(config.DailyConfiguration, nextExecution, hasJumpedDays);
        }

        private DateTime NextExecutionTime(DailyConfiguration dailyConfiguration, DateTime dateTime, bool? hasJumpedDays = false)
        {
            bool hasTimeLimits = dailyConfiguration.TimeLimits != null;
            TimeOnly startTime = hasTimeLimits
                ? dailyConfiguration.TimeLimits.StartTime
                : TimeOnly.MinValue;

            if (dailyConfiguration.Type == DailyConfigType.Once)
            {
                return dateTime.Date.Add(dailyConfiguration.OnceAt.ToTimeSpan());
            }

            if (hasJumpedDays != false && dailyConfiguration.Type == DailyConfigType.Recurring && hasTimeLimits)
            {
                return dateTime.Date.Add(startTime.ToTimeSpan());
            }

            TimeOnly nextExecutionTime = TimeOnly.FromDateTime(dateTime);

            if (Executed)
            {
                nextExecutionTime = AddOccursEveryUnit(dailyConfiguration, nextExecutionTime);
            }

            return nextExecutionTime < startTime
                ? AddStartTime(dailyConfiguration, dateTime)
                : AddNextExecutionTime(dateTime, nextExecutionTime);
        }

        private static DateTime AddNextExecutionTime(DateTime dateTime, TimeOnly nextExecutionTime)
        {
            return dateTime.Date.Add(nextExecutionTime.ToTimeSpan());
        }

        private DateTime AddStartTime(DailyConfiguration dailyConfiguration, DateTime dateTime)
        {
            if (AddOccursEveryUnit(dailyConfiguration, TimeOnly.FromDateTime(dateTime.Date)) < dailyConfiguration.TimeLimits.StartTime)
            {
                return dateTime.Date.Add(dailyConfiguration.TimeLimits.StartTime.ToTimeSpan());
            }
            return dateTime.Date.Add(AddOccursEveryUnit(dailyConfiguration, TimeOnly.FromDateTime(dateTime.Date)).ToTimeSpan());
        }

        private DateTime GetFirstExecutionDate(Configuration config)
        {
            return config.CurrentDate < config.DateLimits.StartDate
                ? config.DateLimits.StartDate
                : config.CurrentDate;
        }

        private void ValidateNextExecutionIsBetweenDateLimits(Configuration config, DateTime dateTime)
        {
            bool startOutOfLimits = dateTime >= config.DateLimits.StartDate;
            bool endOutOfLimits = config.DateLimits.EndDate.HasValue == false || dateTime <= config.DateLimits.EndDate;
            var dateBetweenLimits = startOutOfLimits && endOutOfLimits;

            if (dateTime < config.CurrentDate)
            {
                throw new SchedulerException(_stringLocalizer["Scheduler:Errors:ExecutionEarlierThanCurrentTime"]);
            }

            if (dateBetweenLimits is false)
            {
                throw new SchedulerException(_stringLocalizer["Scheduler:Errors:DateOutOfRanges"]);
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

            List<DayOfWeek> sortedDays = config.WeeklyConfiguration.SelectedDays.OrderBy(_ => _).ToList();
            DayOfWeek actualDayOfWeek = config.CurrentDate.DayOfWeek;
            bool hasLimits = config.DailyConfiguration.TimeLimits != null;
            bool excedsLimits = hasLimits && config.CurrentDate.TimeOfDay >= config.DailyConfiguration.TimeLimits.EndTime.ToTimeSpan();
            bool skipDay = excedsLimits || (config.DailyConfiguration.Type == DailyConfigType.Once && Executed);

            return skipDay
                ? GetNextDayInWeek(sortedDays, actualDayOfWeek, dateTime, config)
                : NextDay(sortedDays, actualDayOfWeek, dateTime);
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

        public DateTime GetNextMonthDate(MonthlyConfiguration monthlyConfiguration, DailyConfiguration dailyConfiguration, DateTime dateTime)
        {
            if (monthlyConfiguration.Type == MonthlyConfigType.DayNumberOption)
            {
                return NextDayInMonth(dateTime, monthlyConfiguration, dailyConfiguration);
            }

            return Executed
                ? ExecutedNextDayOfWeekInMonth(monthlyConfiguration, dailyConfiguration, dateTime)
                : NextDayOfWeekInMonth(monthlyConfiguration, dailyConfiguration, dateTime);
        }

        private DateTime ExecutedNextDayOfWeekInMonth(MonthlyConfiguration monthlyConfiguration, DailyConfiguration dailyConfiguration, DateTime dateTime)
        {
            if (NextExecutionTime(dailyConfiguration, dateTime).TimeOfDay <= dailyConfiguration.TimeLimits.EndTime.ToTimeSpan())
            {
                return dateTime;
            }

            DateTime nextMonthDate = dateTime.AddMonths(monthlyConfiguration.Frecuency).Date;
            dateTime = new DateTime(nextMonthDate.Year, nextMonthDate.Month, 1);

            return NextDayOfWeekInMonth(monthlyConfiguration, dailyConfiguration, dateTime);
        }

        private DateTime NextDayOfWeekInMonth(MonthlyConfiguration monthlyConfig, DailyConfiguration dailyConfig, DateTime currentDate)
        {
            Month month = new(currentDate.Year, currentDate.Month);
            List<DayOfWeek> selectedDays = GetSelectedDays(monthlyConfig);

            bool manyDays = selectedDays.Count != 1;
            TimeSpan endTime = dailyConfig.TimeLimits.EndTime.ToTimeSpan();
            DateTime currentDateTime = currentDate.Date.Add(AddOccursEveryUnit(dailyConfig, TimeOnly.FromDateTime(currentDate)).ToTimeSpan());

            IReadOnlyList<DateTime> listOfDays = month.GetMonthDays()
                .Where(x => x.Date >= currentDate.Date && x.Date.Add(endTime) >= currentDateTime)
                .WhereIf(!manyDays, _ => selectedDays.First() == _.DayOfWeek)
                .WhereIf(manyDays, _ => selectedDays.Contains(_.DayOfWeek))
                .Select(x => x.Add(currentDate.TimeOfDay))
                .ToList();

            return GetSelectedOrdinals(listOfDays, monthlyConfig);
        }

        private List<DayOfWeek> GetSelectedDays(MonthlyConfiguration monthlyConfig)
        {
            return monthlyConfig.SelectedDay switch
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
                _ => throw new SchedulerException(_stringLocalizer["Scheduler:Errors:NotSupportedSelectedWeekDay"]),
            };
        }

        private DateTime GetSelectedOrdinals(IReadOnlyList<DateTime> listOfDays, MonthlyConfiguration monthlyConfig)
        {
            int index = monthlyConfig.OrdinalNumber switch
            {
                Ordinal.First => 0,
                Ordinal.Second => 1,
                Ordinal.Third => 2,
                Ordinal.Fourth => 3,
                Ordinal.Last => listOfDays.Count - 1,
                _ => throw new SchedulerException(_stringLocalizer["Scheduler:Errors:NotSupportedOrdinal"]),
            };

            if (listOfDays.Count - 1 < index)
            {
                throw new SchedulerException(_stringLocalizer["Scheduler:Errors:SelectedDaysIndexOutOfBounds"]);
            }

            return listOfDays[index];
        }

        public DateTime NextDayInMonth(DateTime dateTime, MonthlyConfiguration monthlyConfig, DailyConfiguration dailyConfiguration)
        {
            int dayNumber = monthlyConfig.DayNumber;

            if (dailyConfiguration.Type == DailyConfigType.Once)
            {
                return GetNextPossibleDate(monthlyConfig, dailyConfiguration, dateTime);
            }

            var outOfDate = dateTime > dateTime.JumpToDayNumber(dayNumber);
            var outOfTimeLimits = NextExecutionTime(dailyConfiguration, dateTime).TimeOfDay > dailyConfiguration.TimeLimits.EndTime.ToTimeSpan();
            bool exceedsTheDateTime = outOfDate || outOfTimeLimits;

            if (exceedsTheDateTime)
            {
                dateTime = Executed
                ? dateTime.AddMonths(monthlyConfig.Frecuency + 1).Date
                : dateTime.AddDays(1).Date;
            }

            return dateTime.Day <= dayNumber && dayNumber <= DateTime.DaysInMonth(dateTime.Year, dateTime.Month)
                ? ExcedsDays(dateTime, dayNumber, exceedsTheDateTime)
                : GetNextPossibleDate(monthlyConfig, dailyConfiguration, dateTime);
        }

        private static DateTime ExcedsDays(DateTime dateTime, int dayNumber, bool exceedsTheDateTime)
        {
            DateTime date = dateTime.JumpToDayNumber(dayNumber).Date;
            TimeSpan time = dateTime.TimeOfDay;
            return exceedsTheDateTime
                ? date
                : date.Add(time);
        }

        private DateTime GetNextPossibleDate(MonthlyConfiguration monthlyConfiguration, DailyConfiguration dailyConfiguration, DateTime dateTime)
        {
            bool hasTimeLimits = dailyConfiguration.TimeLimits != null;
            TimeSpan endLimitTime = hasTimeLimits ? dailyConfiguration.TimeLimits.EndTime.ToTimeSpan() : TimeSpan.MinValue;
            TimeSpan nextTime = NextExecutionTime(dailyConfiguration, dateTime).TimeOfDay;
            bool monthHasDayNumber = monthlyConfiguration.DayNumber <= DateTime.DaysInMonth(dateTime.Year, dateTime.Month);

            TimeSpan nextExecutionTime = hasTimeLimits && nextTime > endLimitTime
               ? dailyConfiguration.TimeLimits.StartTime.ToTimeSpan()
               : NextExecutionTime(dailyConfiguration, dateTime).TimeOfDay;

            if (dailyConfiguration.Type == DailyConfigType.Recurring)
            {
                dateTime = dateTime.AddMonths(1);
            }

            if (Executed && monthHasDayNumber && nextExecutionTime > endLimitTime)
            {
                dateTime = dateTime.AddMonths(monthlyConfiguration.Frecuency);
            }

            try
            {
                return new DateTime(dateTime.Year, dateTime.Month, monthlyConfiguration.DayNumber).Add(nextExecutionTime);
            }
            catch (ArgumentOutOfRangeException)
            {
                return new DateTime(dateTime.Year, dateTime.Month + 1, monthlyConfiguration.DayNumber).Add(nextExecutionTime);
            }
        }

        private TimeOnly AddOccursEveryUnit(DailyConfiguration dailyConfiguration, TimeOnly time)
        {
            return dailyConfiguration.DailyFrecuencyType switch
            {
                DailyFrecuency.Hours => time.AddHours(dailyConfiguration.Frecuency.Value),
                DailyFrecuency.Minutes => time.AddMinutes(dailyConfiguration.Frecuency.Value),
                DailyFrecuency.Seconds => time.AddSeconds(dailyConfiguration.Frecuency.Value),
                _ => throw new SchedulerException(_stringLocalizer["DescriptionBuilder:Errors:NotSupportedDailyFrequency"]),
            };
        }
    }
}