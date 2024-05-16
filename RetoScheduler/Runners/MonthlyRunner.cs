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
                    DateTime monthDay;
                    try
                    {
                        monthDay = dateTime.JumpToDayNumber(dayNumber);
                    }
                    catch (Exception)
                    {

                        monthDay = dateTime.JumpToDayNumber(DateTime.DaysInMonth(dateTime.Year, dateTime.Month));
                    }

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
                    else
                    {
                        return monthDay.AddMonths(1).Date;
                    }
                }
            }

            return NextDayOfWeekInMonth(monthlyConfiguration, dateTime, executed);
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
                frecuency = dateTime.Day == 1  /*&& dateTime.Hour ==0*/
                        ? frecuency - 1
                        : frecuency;

                //ejecutar y obtener el siguiente día, si el día actual, es el mismo al proximo día.date, retornar la  fecha actual
                //sino, sumar meses y buscar desde el dia 1
                //SOLUCIONAR ESTA PARTE, EN LA PRIMERA EJECUCION COGE DESDE EL INICIO 1/1 Y EN LA SEGUNDA  COGE DESDE ESA FECHA, POR LO QUE NO PUEDO COMPROBAR
                //NOMBRE DEL TEST Q ESTABA ARREGLANDO Should_Be_Next_Executions_For_Month_WeekDayOption_Third_WeekDay_Skipping_1_Months_With_DailyConfiguration_RecurringType_And_Adding_Hours
                
                
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
                if (selectedOrdinal.Date<dateTime.Date)
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
