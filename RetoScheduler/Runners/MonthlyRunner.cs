using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using RetoScheduler.Extensions;
using System;
using System.Security.Cryptography;

namespace RetoScheduler.Runners
{
    public class MonthlyRunner
    {
        public static DateTime Run(MonthlyConfiguration monthlyConfiguration, DateTime dateTime, bool executed)
        {
            ValidateMonthlyConfiguration(monthlyConfiguration);
            int frecuency = monthlyConfiguration.Frecuency;
            if (monthlyConfiguration.Type == MonthlyConfigType.DayNumberOption)
            {
                return NextMonthlyDayOptionDate(monthlyConfiguration, dateTime, executed, ref frecuency);
            }
            return NextDayOfWeekInMonth(monthlyConfiguration, dateTime, executed);
        }

        private static DateTime NextMonthlyDayOptionDate(MonthlyConfiguration monthlyConfiguration, DateTime dateTime, bool executed, ref int frecuency)
        {
            var dayNumber = monthlyConfiguration.DayNumber;
            int daysInCurrentMonth = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);

            if (executed)
            {
                frecuency -= dateTime.Day == 1
                        ? 1 : 0;
                var nextMonth = dateTime.AddMonths(frecuency).Date;
                int daysInNextMonth = DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);

                return dayNumber > daysInCurrentMonth || dateTime.Date != dateTime.JumpToDayNumber(dayNumber).Date
                ? JumpToLowest(nextMonth, dayNumber, daysInNextMonth)
                : dateTime;
            }
            else
            {
                DateTime monthDay = JumpToLowest(dateTime, dayNumber, daysInCurrentMonth);

                return monthDay >= dateTime.Date
                    ? GetDateInThisMonth(dateTime, monthDay)
                    : monthDay.AddMonths(1);
            }
        }

        private static DateTime JumpToLowest(DateTime dateTime, int dayNumber, int daysInNextMonth)
        {
            int validDayNumber = Math.Min(dayNumber, daysInNextMonth);
            return dateTime.JumpToDayNumber(validDayNumber).Date;
        }

        private static DateTime GetDateInThisMonth(DateTime dateTime, DateTime monthDay)
        {
            return monthDay == dateTime.Date
                ? dateTime
                : monthDay;
        }

        private static void ValidateMonthlyConfiguration(MonthlyConfiguration monthlyConfiguration)
        {
            if (monthlyConfiguration.Frecuency < 1)
            {
                throw new SchedulerException("The month frequency can't be zero or negative");
            }
        }

        private static DateTime NextDayOfWeekInMonth(MonthlyConfiguration monthlyConfig, DateTime dateTime, bool executed)
        {
            int frecuency = monthlyConfig.Frecuency;

            List<DayOfWeek> selectedDays = GetSelectedDays(monthlyConfig);
            bool manyDays = selectedDays.Count != 1;

            if (executed)
            {
                frecuency -= dateTime.Day == 1
                        ? 1 : 0;

                Month executedMonth = new Month(dateTime.Year, dateTime.Month);
                IReadOnlyList<DateTime> listOfDays = executedMonth.GetMonthDays()
                .WhereIf(!manyDays, _ => selectedDays.First() == _.DayOfWeek)
                .WhereIf(manyDays, _ => selectedDays.Contains(_.DayOfWeek))
                .Select(x => x)
                .ToList();

                var selectedOrdinal = GetSelectedOrdinals(listOfDays, monthlyConfig);
                if (selectedOrdinal.Date == dateTime.Date)
                {
                    return dateTime;
                }
                else
                {
                    var addingMonths = dateTime.AddMonths(frecuency);
                    executedMonth = new Month(addingMonths.Year, addingMonths.Month);
                    listOfDays = executedMonth.GetMonthDays()
                   .WhereIf(!manyDays, _ => selectedDays.First() == _.DayOfWeek)
                   .WhereIf(manyDays, _ => selectedDays.Contains(_.DayOfWeek))
                   .Select(x => x.Date)
                   .ToList();
                    selectedOrdinal = GetSelectedOrdinals(listOfDays, monthlyConfig);

                }
                return selectedOrdinal;
            }
            else
            {
                Month month = new(dateTime.Year, dateTime.Month);

                IReadOnlyList<DateTime> listOfDays = month.GetMonthDays()
                    .WhereIf(!manyDays, _ => selectedDays.First() == _.DayOfWeek)
                    .WhereIf(manyDays, _ => selectedDays.Contains(_.DayOfWeek))
                    .Select(x => x)
                    .ToList();

                var selectedOrdinal = GetSelectedOrdinals(listOfDays, monthlyConfig);
                if (selectedOrdinal.Date == dateTime.Date)
                {
                    return dateTime;
                }
                if (selectedOrdinal.Date < dateTime.Date)
                {
                    var nextMonth = new Month(dateTime.AddMonths(1).Year, dateTime.AddMonths(1).Month);
                    listOfDays = nextMonth.GetMonthDays()
                    .WhereIf(!manyDays, _ => selectedDays.First() == _.DayOfWeek)
                    .WhereIf(manyDays, _ => selectedDays.Contains(_.DayOfWeek))
                    .Select(x => x)
                    .ToList();
                }

                return GetSelectedOrdinals(listOfDays, monthlyConfig);
            }
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
