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
            var date = dateTime.Date;
            int frecuency = monthlyConfiguration.Frecuency;
            if (monthlyConfiguration.Type == MonthlyConfigType.DayNumberOption)
            {
                var dayNumber = monthlyConfiguration.DayNumber;

                if (executed)
                {
                    try
                    {
                        frecuency = dateTime.Day == 1
                        ? frecuency - 1
                        : frecuency;

                        DateTime monthDay = dateTime.JumpToDayNumber(dayNumber);
                        if (dateTime.Date == monthDay.Date)
                        {
                            return dateTime;
                        }
                        return dateTime.AddMonths(frecuency).JumpToDayNumber(dayNumber).Date;
                    }
                    catch (Exception)
                    {
                        var daysInMonth = DateTime.DaysInMonth(dateTime.AddMonths(frecuency).Year, dateTime.AddMonths(frecuency).Month);
                        daysInMonth = daysInMonth > dayNumber
                            ? dayNumber
                            : daysInMonth;
                        return dateTime.AddMonths(frecuency).JumpToDayNumber(daysInMonth).Date;
                    }
                }
                else
                {
                    if (!executed && dateTime.TimeOfDay == new TimeSpan(0, 0, 0))
                    {
                        try
                        {
                            return dateTime.JumpToDayNumber(dayNumber).Date;
                        }
                        catch (Exception)
                        {

                            return dateTime.JumpToDayNumber(DateTime.DaysInMonth(dateTime.Year, dateTime.Month)).Date;
                        }
                    }

                    DateTime monthDay = dateTime.JumpToDayNumber(dayNumber);

                    if (monthDay.Date == dateTime.Date)
                    {
                        return dateTime;
                    }

                    //si monthDay es mayor o igual a dateTime, 
                    var monthDayLessThanCurrentDate = monthDay < dateTime;
                    if (!monthDayLessThanCurrentDate)
                    {
                        return monthDay.Date;
                    }
                    return monthDay.AddMonths(1).Date;
                }
            }

            return NextDayOfWeekInMonth(monthlyConfiguration, date, executed);
        }

        private static void ValidateMonthlyConfiguration(MonthlyConfiguration monthlyConfiguration)
        {
            if (monthlyConfiguration.Frecuency < 1)
            {
                throw new SchedulerException("The month frequency can't be zero or negative");
            }
        }

        private static Month GetNextPossibleMonth(DateTime currentDate, int dayNumber)
        {
            var nextMonth1 = currentDate.AddMonths(0);
            var nextMonth2 = currentDate.AddMonths(1);
            var nextMonth3 = currentDate.AddMonths(2);

            List<Month> nextMonths = new List<Month>() {
                new Month(nextMonth1.Year, nextMonth1.Month),
                new Month(nextMonth2.Year, nextMonth2.Month),
                new Month(nextMonth3.Year, nextMonth3.Month) };

            var possibleMonth = nextMonths.
                Where(x => x.GetMonthDays().Count >= dayNumber && new DateTime(x.Year, x.MonthIndex, dayNumber, currentDate.Hour, currentDate.Minute, currentDate.Second) >= currentDate)
                .Select(x => x)
                .First();

            return possibleMonth;
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
