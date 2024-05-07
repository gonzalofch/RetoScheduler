using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using RetoScheduler.Extensions;

namespace RetoScheduler.Runners
{
    public class MonthlyRunner
    {
        public static DateTime Run(MonthlyConfiguration monthlyConfiguration,DailyConfiguration dailyConfiguration, DateTime dateTime, bool executed)
        {
            if (monthlyConfiguration.Type == MonthlyConfigType.DayNumberOption)
            {
                return NextDayInMonth(dateTime, monthlyConfiguration, dailyConfiguration, executed);
            }

            return executed
                ? ExecutedNextDayOfWeekInMonth(monthlyConfiguration, dailyConfiguration, dateTime,executed)
                : NextDayOfWeekInMonth(monthlyConfiguration, dailyConfiguration, dateTime);
        }

        public static DateTime NextDayInMonth(DateTime dateTime, MonthlyConfiguration monthlyConfig, DailyConfiguration dailyConfiguration,bool executed)
        {
            int dayNumber = monthlyConfig.DayNumber;

            if (dailyConfiguration.Type == DailyConfigType.Once)
            {
                return GetNextPossibleDate(monthlyConfig, dailyConfiguration, dateTime, executed);
            }

            var outOfDate = dateTime > dateTime.JumpToDayNumber(dayNumber);
            var outOfTimeLimits = NextExecutionTime(dailyConfiguration, dateTime, executed).TimeOfDay > dailyConfiguration.TimeLimits.EndTime.ToTimeSpan();
            bool exceedsTheDateTime = outOfDate || outOfTimeLimits;

            if (exceedsTheDateTime)
            {
                dateTime = executed
                ? dateTime.AddMonths(monthlyConfig.Frecuency + 1).Date
                : dateTime.AddDays(1).Date;
            }

            return dateTime.Day <= dayNumber && dayNumber <= DateTime.DaysInMonth(dateTime.Year, dateTime.Month)
                ? ExcedsDays(dateTime, dayNumber, exceedsTheDateTime)
                : GetNextPossibleDate(monthlyConfig, dailyConfiguration, dateTime,executed);
        }

        private static DateTime GetNextPossibleDate(MonthlyConfiguration monthlyConfiguration, DailyConfiguration dailyConfiguration, DateTime dateTime, bool executed)
        {
            bool hasTimeLimits = dailyConfiguration.TimeLimits != null;
            TimeSpan endLimitTime = hasTimeLimits ? dailyConfiguration.TimeLimits.EndTime.ToTimeSpan() : TimeSpan.MinValue;
            TimeSpan nextTime = NextExecutionTime(dailyConfiguration, dateTime, executed).TimeOfDay;
            bool monthHasDayNumber = monthlyConfiguration.DayNumber <= DateTime.DaysInMonth(dateTime.Year, dateTime.Month);

            TimeSpan nextExecutionTime = hasTimeLimits && nextTime > endLimitTime
               ? dailyConfiguration.TimeLimits.StartTime.ToTimeSpan()
               : NextExecutionTime(dailyConfiguration, dateTime, executed).TimeOfDay;

            if (dailyConfiguration.Type == DailyConfigType.Recurring)
            {
                dateTime = dateTime.AddMonths(1);
            }

            if (executed && monthHasDayNumber && nextExecutionTime > endLimitTime)
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

        private static DateTime ExecutedNextDayOfWeekInMonth(MonthlyConfiguration monthlyConfiguration, DailyConfiguration dailyConfiguration, DateTime dateTime, bool executed)
        {
            if (NextExecutionTime(dailyConfiguration, dateTime,executed).TimeOfDay <= dailyConfiguration.TimeLimits.EndTime.ToTimeSpan())
            {
                return dateTime;
            }

            DateTime nextMonthDate = dateTime.AddMonths(monthlyConfiguration.Frecuency).Date;
            dateTime = new DateTime(nextMonthDate.Year, nextMonthDate.Month, 1);

            return NextDayOfWeekInMonth(monthlyConfiguration, dailyConfiguration, dateTime);
        }

        private static DateTime ExcedsDays(DateTime dateTime, int dayNumber, bool exceedsTheDateTime)
        {
            DateTime date = dateTime.JumpToDayNumber(dayNumber).Date;
            TimeSpan time = dateTime.TimeOfDay;
            return exceedsTheDateTime
                ? date
                : date.Add(time);
        }

        private static DateTime GetSelectedOrdinals(IReadOnlyList<DateTime> listOfDays, MonthlyConfiguration monthlyConfig)
        {
            int index = monthlyConfig.OrdinalNumber switch
            {
                Ordinal.First => 0,
                Ordinal.Second => 1,
                Ordinal.Third => 2,
                Ordinal.Fourth => 3,
                Ordinal.Last => listOfDays.Count - 1,
                _ => throw new SchedulerException("Scheduler:Errors:NotSupportedOrdinal"),
            };

            if (listOfDays.Count - 1 < index)
            {
                throw new SchedulerException("Scheduler:Errors:SelectedDaysIndexOutOfBounds");
            }


            return listOfDays[index];
        }

        private static DateTime NextDayOfWeekInMonth(MonthlyConfiguration monthlyConfig, DailyConfiguration dailyConfig, DateTime currentDate)
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
                _ => throw new SchedulerException("Scheduler:Errors:NotSupportedSelectedWeekDay"),
            };
        }
        private static DateTime NextExecutionTime(DailyConfiguration dailyConfiguration, DateTime dateTime,bool executed, bool? hasJumpedDays = false)
        {
            //AQUI UTILIZAR DAILYRUNNER PARA QUE FUNCIONE DE IGUAL MANERA
            bool hasTimeLimits = dailyConfiguration.TimeLimits != null;
            TimeOnly startTime = hasTimeLimits
                ? dailyConfiguration.TimeLimits.StartTime
                : TimeOnly.MinValue;

            if (hasJumpedDays == true && dailyConfiguration.Type == DailyConfigType.Recurring && hasTimeLimits)
            {
                return dateTime.Date.Add(startTime.ToTimeSpan());
            }

            return DailyRunner.Run(dailyConfiguration, dateTime, executed);
        }
        public static TimeOnly AddOccursEveryUnit(DailyConfiguration dailyConfiguration, TimeOnly time)
        {
            return dailyConfiguration.DailyFrecuencyType switch
            {
                DailyFrecuency.Hours => time.AddHours(dailyConfiguration.Frecuency.Value),
                DailyFrecuency.Minutes => time.AddMinutes(dailyConfiguration.Frecuency.Value),
                DailyFrecuency.Seconds => time.AddSeconds(dailyConfiguration.Frecuency.Value),
                _ => throw new SchedulerException("DescriptionBuilder:Errors:NotSupportedDailyFrequency"),
            };
        }
    }
}
