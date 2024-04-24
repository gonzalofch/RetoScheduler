using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using RetoScheduler.Extensions;
using System.Text;

namespace RetoScheduler
{
    public class Scheduler
    {
        private const string Space = " ";

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
                throw new SchedulerException("You need to check field to Run Program");
            }
        }

        private static void ValidateLimitsRange(Configuration config)
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

        private static string CalculateDescription(DateTime dateTime, Configuration config)
        {
            StringBuilder description = new StringBuilder("Occurs ");

            if (config.Type == ConfigType.Once && config.ConfigDateTime.HasValue)
            {
                description.Append(GetOnceAtDescription(dateTime));
            }
            else
            {
                string configurationTypeDescription = config.MonthlyConfiguration != null
                    ? GetMonthlyDescription(config)
                    : GetWeeklyDescription(config);

                description.Append(configurationTypeDescription);
            }

            description.Append(GetDailyDescription(config));
            description.Append(GetDateLimitsDescription(config));

            return description.ToString();
        }

        private static string GetOnceAtDescription(DateTime dateTime)
        {
            return "once at " + dateTime.ToString("dd/MM/yyyy ");
        }

        private static string GetDateLimitsDescription(Configuration config)
        {
            string startDate = config.DateLimits.StartDate.Date.ToString("dd/MM/yyyy");
            string endDate = config.DateLimits.EndDate?.ToString("dd/MM/yyyy");
            return config.DateLimits.EndDate.HasValue
                ? "starting on " + startDate + " and finishing on " + endDate
                : "starting on " + startDate;
        }

        private static string GetMonthlyDescription(Configuration config)
        {
            string monthlyDescription = "the ";

            monthlyDescription += config.MonthlyConfiguration.Type == MonthlyConfigType.DayNumberOption
                ? GetMonthlyDayOfNumber(config)
                : GetMonthlyWeekdaysMessage(config);

            monthlyDescription += "of very ";
            monthlyDescription += GetMonthlyFrecuencyMessage(config);

            return monthlyDescription;
        }

        private static string GetMonthlyDayOfNumber(Configuration config)
        {
            return config.MonthlyConfiguration.DayNumber switch
            {
                1 or 21 or 31 => config.MonthlyConfiguration.DayNumber + "st ",
                2 => "2nd ",
                3 => "3rd ",
                > 3 and < 32 => config.MonthlyConfiguration.DayNumber + "th ",
                _ => throw new SchedulerException("Not supported action for monthly day number message"),
            };
        }

        private static string GetMonthlyWeekdaysMessage(Configuration config)
        {
            string ordinal = config.MonthlyConfiguration.OrdinalNumber.ToString().ToLower() + " ";
            string selectedWeekDay = config.MonthlyConfiguration.SelectedDay.ToString().ToLower() + " ";

            return ordinal + selectedWeekDay;
        }

        private static string GetMonthlyFrecuencyMessage(Configuration config)
        {
            return config.MonthlyConfiguration.Frecuency switch
            {
                0 => "months ",
                1 => config.MonthlyConfiguration.Frecuency + " month ",
                > 1 => config.MonthlyConfiguration.Frecuency + " months ",
                _ => throw new SchedulerException("Not supported action for monthly frecuency message"),
            };
        }

        private static string GetWeeklyDescription(Configuration config)
        {
            return config.WeeklyConfiguration != null
                ? "every " + GetWeeklyFrecuencyMessage(config)
                : "every day ";
        }

        private static string GetWeeklyFrecuencyMessage(Configuration config)
        {
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

            if (config.DailyConfiguration.Type == DailyConfigType.Once && config.DailyConfiguration.OnceAt != TimeOnly.MinValue)
            {
                string dailyExecutionTime = config.DailyConfiguration.OnceAt.ParseAmPm();
                return "one time at " + dailyExecutionTime + Space;
            }

            var limits = config.DailyConfiguration.TimeLimits;
            string dailyDescription = config.WeeklyConfiguration == null ? "and " : string.Empty;
            string timeStartLimit = limits.StartTime.ParseAmPm();
            string timeEndLimit = limits.EndTime.ParseAmPm();

            return dailyDescription + GetDailyFrecuencyMessage(config) + timeStartLimit + " and " + timeEndLimit + Space;
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

            return "every " + config.DailyConfiguration.Frecuency + Space + timeUnit + " between ";
        }

        private static string GetStringListDayOfWeek(List<DayOfWeek> selectedDays)
        {
            string formattedList = "on";

            foreach (var item in selectedDays)
            {
                string dayInLower = item.ToString().ToLower();
                if (item == selectedDays.Last() && selectedDays.Count() >= 2)
                {
                    formattedList += " and " + dayInLower + Space;
                }
                else
                {
                    formattedList += item == selectedDays.First()
                        ? Space
                        : ", ";

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

            TimeSpan addedTime = config.DailyConfiguration.Type == DailyConfigType.Once
                ? config.DailyConfiguration.OnceAt.ToTimeSpan()
                : config.DailyConfiguration.TimeLimits.StartTime.ToTimeSpan();

            return config.ConfigDateTime.Value + addedTime;
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

            var sortedDays = config.WeeklyConfiguration.SelectedDays.OrderBy(_ => _).ToList();
            var actualDay = config.CurrentDate.DayOfWeek;
            var hasLimits = config.DailyConfiguration.TimeLimits != null;
            var excedsLimits = hasLimits && config.CurrentDate.TimeOfDay >= config.DailyConfiguration.TimeLimits.EndTime.ToTimeSpan();
            var skipDay = excedsLimits || (config.DailyConfiguration.Type == DailyConfigType.Once && Executed);

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
            if (monthlyConfiguration.Type == MonthlyConfigType.DayNumberOption)
            {
                return NextDayInMonth(dateTime, monthlyConfiguration, dailyConfiguration);
            }

            return Executed
                ? ExecutedNextDayOfWeekInMonth(monthlyConfiguration, dailyConfiguration, dateTime)
                : NextDayOfWeekInMonth(monthlyConfiguration, dateTime);
        }

        private DateTime ExecutedNextDayOfWeekInMonth(MonthlyConfiguration monthlyConfiguration, DailyConfiguration dailyConfiguration, DateTime dateTime)
        {
            if (NextExecutionTime(dailyConfiguration, dateTime).TimeOfDay <= dailyConfiguration.TimeLimits.EndTime.ToTimeSpan())
            {
                return dateTime;
            }

            DateTime nextMonthDate = dateTime.AddMonths(monthlyConfiguration.Frecuency);
            dateTime = new DateTime(nextMonthDate.Year, nextMonthDate.Month, 1);

            return NextDayOfWeekInMonth(monthlyConfiguration, dateTime);
        }

        private static DateTime NextDayOfWeekInMonth(MonthlyConfiguration monthlyConfig, DateTime currentDate)
        {
            Month month = new Month(currentDate.Year, currentDate.Month);
            List<DayOfWeek> selectedDays = GetSelectedDays(monthlyConfig);

            bool manyDays = selectedDays.Count != 1;

            IReadOnlyList<DateTime> listOfDays = month.GetMonthDays()
                .Where(x => x >= currentDate)
                .WhereIf(!manyDays, _ => selectedDays.First() == _.DayOfWeek)
                .Select(x => x.Add(currentDate.TimeOfDay))
                .WhereIf(manyDays, _ => selectedDays.Contains(_.DayOfWeek))
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
                _ => throw new Exception("Exception for index in list"),
            };

            if (list.Count < index)
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
            try
            {
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
            catch (Exception)
            {
                return GetNextPossibleDate(monthlyConfig, dailyConfiguration, dateTime);
            }
        }

        private static DateTime ExcedsDays(DateTime dateTime, int dayNumber, bool exceedsTheDateTime)
        {
            DateTime date = dateTime.JumpToDayNumber(dayNumber).Date;
            TimeSpan time = dateTime.TimeOfDay;
            return exceedsTheDateTime
                ? date
                : date + time;
        }

        private DateTime GetNextPossibleDate(MonthlyConfiguration monthlyConfiguration, DailyConfiguration dailyConfiguration, DateTime dateTime)
        {
            var nextExecutionTime = NextExecutionTime(dailyConfiguration, dateTime).TimeOfDay;
            int daysInMonth = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
            bool isRecurringType = dailyConfiguration.Type == DailyConfigType.Recurring;

            if (isRecurringType)
            {
                dateTime = dateTime.AddMonths(1);
            }

            if (monthlyConfiguration.DayNumber <= daysInMonth && Executed)
            {
                if (isRecurringType && nextExecutionTime > dailyConfiguration.TimeLimits.EndTime.ToTimeSpan())
                {
                    dateTime = dateTime.AddMonths(monthlyConfiguration.Frecuency);
                }
                else if (!isRecurringType)
                {
                    dateTime = dateTime.AddMonths(monthlyConfiguration.Frecuency);
                }
            }

            try
            {
                return new DateTime(dateTime.Year, dateTime.Month, monthlyConfiguration.DayNumber);
            }
            catch (ArgumentOutOfRangeException)
            {
                return new DateTime(dateTime.Year, dateTime.Month + 1, monthlyConfiguration.DayNumber);
            }
        }

        private static TimeOnly AddOccursEveryUnit(DailyConfiguration dailyConfiguration, TimeOnly dateTimeTime)
        {
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