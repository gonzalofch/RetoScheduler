using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using RetoScheduler.Extensions;
using System.Globalization;

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
            Thread.CurrentThread.CurrentCulture = new CultureInfo(config.Cultures.GetDescription());
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(config.Cultures.GetDescription());

            ValidateConfiguration(config);

            DateTime dateTime = config.Type == ConfigType.Once
                ? InOnce(config)
                : InRecurring(config);

            ValidateNextExecutionIsBetweenDateLimits(config, dateTime);
            Executed = true;
            DescriptionBuilder builder = new();
            string description = builder.CalculateDescription(dateTime, config);

            return new OutPut(dateTime, description);
        }

        private static void ValidateConfiguration(Configuration config)
        {
            ValidadIsEnabled(config);
            ValidateLimitsRange(config);
            ValidateLimitsTimeRange(config);
        }

        private static void ValidadIsEnabled(Configuration config)
        {
            if (!config.Enabled)
            {
                throw new SchedulerException("You need to check field to run the Scheduler");
            }
        }

        private static void ValidateLimitsRange(Configuration config)
        {
            if (config.DateLimits == null)
            {
                throw new SchedulerException("Limits Can`t be null");
            }

            if (config.DateLimits.EndDate < config.DateLimits.StartDate)
            {
                throw new SchedulerException("The end date cannot be earlier than the initial date");
            }
        }

        private static void ValidateLimitsTimeRange(Configuration config)
        {
            bool isRecurring = config.DailyConfiguration.Type == DailyConfigType.Recurring;
            bool hasTimeLimits = config.DailyConfiguration.TimeLimits != null;
            bool endTimeIsShorter = isRecurring && config.DailyConfiguration.TimeLimits.EndTime < config.DailyConfiguration.TimeLimits.StartTime;
            if (hasTimeLimits && endTimeIsShorter)
            {
                throw new SchedulerException("The EndTime cannot be earlier than StartTime");
            }
        }

        private static DateTime InOnce(Configuration config)
        {
            if (config.ConfigDateTime.HasValue == false)
            {
                throw new SchedulerException("Once Types requires an obligatory DateTime");
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
                throw new SchedulerException("Don't should put negative numbers or zero in number field");
            }
            var dateTime = GetFirstExecutionDate(config);
            dateTime = NextExecutionDate(config, dateTime);

            bool jumpDay = config.CurrentDate.Date != dateTime.Date;

            return NextExecutionTime(config.DailyConfiguration, dateTime, jumpDay);
        }

        private DateTime NextExecutionTime(DailyConfiguration dailyConfiguration, DateTime dateTime, bool? jumpDay = false)
        {

            if (jumpDay != false && dailyConfiguration.Type == DailyConfigType.Recurring && dailyConfiguration.TimeLimits != null)
            {
                return dateTime.Date + dailyConfiguration.TimeLimits.StartTime.ToTimeSpan();
            }

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
            if (AddOccursEveryUnit(dailyConfiguration, TimeOnly.FromDateTime(dateTime.Date)) < dailyConfiguration.TimeLimits.StartTime)
            {
                return dateTime.Date + dailyConfiguration.TimeLimits.StartTime.ToTimeSpan();

            }
            return dateTime.Date + AddOccursEveryUnit(dailyConfiguration, TimeOnly.FromDateTime(dateTime.Date)).ToTimeSpan();
        }

        private DateTime GetFirstExecutionDate(Configuration config)
        {
            return config.CurrentDate < config.DateLimits.StartDate
                ? config.DateLimits.StartDate
                : config.CurrentDate;
        }

        private static void ValidateNextExecutionIsBetweenDateLimits(Configuration config, DateTime dateTime)
        {
            bool startOutOfLimits = dateTime >= config.DateLimits.StartDate;
            bool endOutOfLimits = config.DateLimits.EndDate.HasValue == false || dateTime <= config.DateLimits.EndDate;
            var dateBetweenLimits = startOutOfLimits && endOutOfLimits;

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

        private static DateTime NextDayOfWeekInMonth(MonthlyConfiguration monthlyConfig, DailyConfiguration dailyConfig, DateTime currentDate)
        {
            Month month = new Month(currentDate.Year, currentDate.Month);
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

            return ObtainOrdinalsFromList(listOfDays, monthlyConfig);
        }

        private static List<DayOfWeek> GetSelectedDays(MonthlyConfiguration monthlyConfig)
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
                _ => throw new SchedulerException("The selected Kind of Day is not supported"),
            };
        }

        private static DateTime ObtainOrdinalsFromList(IReadOnlyList<DateTime> list, MonthlyConfiguration monthlyConfig)
        {
            int index = monthlyConfig.OrdinalNumber switch
            {
                Ordinal.First => 0,
                Ordinal.Second => 1,
                Ordinal.Third => 2,
                Ordinal.Fourth => 3,
                Ordinal.Last => list.Count() - 1,
                _ => throw new SchedulerException("Selected Ordinal is not supported"),
            };

            if (list.Count - 1 < index)
            {
                throw new SchedulerException("The index is greater than the number of days");
            }

            return list[index];
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
            bool hasLimits = dailyConfiguration.TimeLimits != null;
            TimeSpan endTime = hasLimits ? dailyConfiguration.TimeLimits.EndTime.ToTimeSpan() : TimeSpan.MinValue;
            TimeSpan nextTime = NextExecutionTime(dailyConfiguration, dateTime).TimeOfDay;
            bool monthHasDayNumber = monthlyConfiguration.DayNumber <= DateTime.DaysInMonth(dateTime.Year, dateTime.Month);


            TimeSpan nextExecutionTime = hasLimits && nextTime > endTime
               ? dailyConfiguration.TimeLimits.StartTime.ToTimeSpan()
               : NextExecutionTime(dailyConfiguration, dateTime).TimeOfDay;

            if (dailyConfiguration.Type == DailyConfigType.Recurring)
            {
                dateTime = dateTime.AddMonths(1);
            }

            if (Executed && monthHasDayNumber && nextExecutionTime > endTime)
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

        private static TimeOnly AddOccursEveryUnit(DailyConfiguration dailyConfiguration, TimeOnly dateTimeTime)
        {
            return dailyConfiguration.DailyFrecuencyType switch
            {
                DailyFrecuency.Hours => dateTimeTime.AddHours(dailyConfiguration.Frecuency.Value),
                DailyFrecuency.Minutes => dateTimeTime.AddMinutes(dailyConfiguration.Frecuency.Value),
                DailyFrecuency.Seconds => dateTimeTime.AddSeconds(dailyConfiguration.Frecuency.Value),
                _ => throw new SchedulerException("Daily Frequency Type is Not supported"),
            };
        }
    }
}