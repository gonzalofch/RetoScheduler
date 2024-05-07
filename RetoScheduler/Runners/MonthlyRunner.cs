using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using RetoScheduler.Extensions;
using System;

namespace RetoScheduler.Runners
{
    public class MonthlyRunner
    {
        public static DateTime Run(MonthlyConfiguration monthlyConfiguration, DateTime dateTime, bool executed)
        {

            if (monthlyConfiguration.Type == MonthlyConfigType.DayNumberOption)
            {
                return NextDayInMonth(dateTime, monthlyConfiguration, executed);
            }
            else
            {
                return NextDayOfWeekInMonth(monthlyConfiguration, dateTime, executed);
            }

        }

        public static DateTime NextDayInMonth(DateTime dateTime, MonthlyConfiguration monthlyConfig, bool executed)
        {
            dateTime = executed
                ? dateTime.AddMonths(monthlyConfig.Frecuency)
                : dateTime;

            int dayNumber = monthlyConfig.DayNumber;
            return dateTime.Day <= dayNumber && dayNumber <= DateTime.DaysInMonth(dateTime.Year, dateTime.Month)
                ? dateTime.JumpToDayNumber(dayNumber)
                : GetNextPossibleDate(dateTime.AddMonths(1), dayNumber);//cambiar esto luego de que funcionen todos los tests
        }

        private static DateTime GetNextPossibleDate(DateTime currentDate, int dayNumber)
        {
            try
            {

                return new DateTime(currentDate.Year, currentDate.Month, dayNumber);
            }
            catch (ArgumentOutOfRangeException)
            {
                return new DateTime(currentDate.Year, currentDate.Month+1, dayNumber);
            }
        }

        private static DateTime NextDayOfWeekInMonth(MonthlyConfiguration monthlyConfig, DateTime currentDate, bool executed)
        {
            currentDate = executed
                 ? new DateTime(currentDate.Year, currentDate.Month, 1).AddMonths(monthlyConfig.Frecuency)
                 : currentDate;

            Month month = new(currentDate.Year, currentDate.Month);
            List<DayOfWeek> selectedDays = GetSelectedDays(monthlyConfig);

            bool manyDays = selectedDays.Count != 1;

            IReadOnlyList<DateTime> listOfDays = month.GetMonthDays()
                .Where(x => x.Date >= currentDate.Date)
                .WhereIf(!manyDays, _ => selectedDays.First() == _.DayOfWeek)
                .WhereIf(manyDays, _ => selectedDays.Contains(_.DayOfWeek))
                .Select(x => x/*.Add(currentDate.TimeOfDay)*/)
                .ToList();

            return GetSelectedOrdinals(listOfDays, monthlyConfig);
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
    }
}
