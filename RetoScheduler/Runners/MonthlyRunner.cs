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
            return monthlyConfiguration.Type == MonthlyConfigType.DayNumberOption
                ? NextMonthlyDayOptionDate(monthlyConfiguration, dateTime, executed)
                : NextDayOfWeekInMonth(monthlyConfiguration, dateTime, executed);
        }

        private static DateTime NextMonthlyDayOptionDate(MonthlyConfiguration monthlyConfiguration, DateTime dateTime, bool executed)
        {

            var dayNumber = monthlyConfiguration.DayNumber;
            int daysInCurrentMonth = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);

            if (executed)
            {
                var nextMonth = GetNextMonth(dateTime, monthlyConfiguration.Frecuency).Date;
                int daysInNextMonth = DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);
                int validDayNumber = Math.Min(dayNumber, daysInNextMonth);

                //va a pasar, que llega el mismo datetime que el jumptodaynumber
                //va a pasar que llega es diferente del jumptodaynumber
                if (dateTime.Day > dayNumber)
                {
                    if (daysInNextMonth >= dayNumber)
                    {
                        return nextMonth.JumpToDayNumber(dayNumber).Date;

                    }
                    return nextMonth.JumpToDayNumber(daysInNextMonth).Date;
                }

                if (dateTime.Day < dayNumber)
                {
                    //aqui entra
                    if (daysInCurrentMonth >= dayNumber)
                    {
                        return nextMonth.JumpToDayNumber(dayNumber).Date;
                    }
                    return nextMonth.JumpToDayNumber(daysInNextMonth).Date;
                }

                if (dateTime == dateTime.JumpToDayNumber(dayNumber).Add(dateTime.TimeOfDay))
                {
                    return dateTime;
                }
                return dateTime;
            }
            else
            {
                var nextMonth = GetNextMonth(dateTime).Date;
                int daysInNextMonth = DateTime.DaysInMonth(nextMonth.Year, nextMonth.Month);

                if (dateTime.Day < dayNumber)
                {
                    if (daysInCurrentMonth >= dayNumber)
                    {
                        return dateTime.JumpToDayNumber(dayNumber).Date;
                    }
                    return dateTime.JumpToDayNumber(daysInCurrentMonth).Date;
                }


                if (dateTime.Day > dayNumber)
                {
                    if (daysInNextMonth >= dayNumber)
                    {
                        return nextMonth.JumpToDayNumber(dayNumber).Date;

                    }
                    return nextMonth.JumpToDayNumber(daysInNextMonth).Date;
                }

                if (dateTime == dateTime.JumpToDayNumber(dayNumber).Add(dateTime.TimeOfDay))
                {
                    return dateTime;
                }

                return dateTime;
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
            Month month = new(dateTime.Year, dateTime.Month);
            List<DayOfWeek> selectedDays = GetSelectedDays(monthlyConfig);
            bool manyDays = selectedDays.Count != 1;
            IReadOnlyList<DateTime> listOfDays = GetListOfSelectedDays(month, selectedDays, manyDays);
            var selectedOrdinal = GetSelectedOrdinals(listOfDays, monthlyConfig);
            DateTime dateWithAddedMonths = dateTime;

            if (selectedOrdinal.Date == dateTime.Date)
            {
                return dateTime;
            }
            if (executed)
            {
                frecuency -= dateTime.Day == 1
                        ? 1 : 0;
                dateWithAddedMonths = GetNextMonth(dateTime, frecuency);
                month = new Month(dateWithAddedMonths.Year, dateWithAddedMonths.Month);

                listOfDays = GetListOfSelectedDays(month, selectedDays, manyDays);

                var selectedOrdinalExec = GetSelectedOrdinals(listOfDays, monthlyConfig);
                if (selectedOrdinal != selectedOrdinalExec)
                {
                    return selectedOrdinalExec;
                }
            }
            if (!executed && selectedOrdinal < dateTime)
            {
                dateWithAddedMonths = GetNextMonth(dateTime);
            }

            month = new Month(dateWithAddedMonths.Year, dateWithAddedMonths.Month);
            listOfDays = GetListOfSelectedDays(month, selectedDays, manyDays);


            return GetSelectedOrdinals(listOfDays, monthlyConfig);

        }

        private static DateTime GetNextMonth(DateTime dateTime, int frecuency = 1)
        {
            if (frecuency != 1)
            {
                return dateTime.AddMonths(frecuency).Date;
            }
            return dateTime.AddMonths(frecuency);
        }

        private static IReadOnlyList<DateTime> GetListOfSelectedDays(Month month, List<DayOfWeek> selectedDays, bool manyDays)
        {
            return month.GetMonthDays()
                           .WhereIf(!manyDays, _ => selectedDays.First() == _.DayOfWeek)
                           .WhereIf(manyDays, _ => selectedDays.Contains(_.DayOfWeek))
                           .Select(x => x)
                           .ToList();
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
