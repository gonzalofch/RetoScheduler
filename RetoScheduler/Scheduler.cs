using Microsoft.VisualBasic;
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
                if (config.DailyConfiguration.TimeLimits.EndTime.ToTimeSpan() < config.CurrentDate.TimeOfDay)
                {
                    throw new SchedulerException("The EndTime cannot be earlier than Current Time");
                }

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

            var dateTime = Executed
                ? config.CurrentDate
                : GetFirstExecutionDate(config);

            dateTime = NextDayExecution(config, dateTime);

            return NextExecutionTime(config, dateTime);
        }

        private DateTime NextExecutionTime(Configuration config, DateTime dateTime)
        {
            //revisar si es once o si es recurring
            TimeOnly dateTimeTime = TimeOnly.FromDateTime(dateTime);

            if (config.DailyConfiguration.Type == DailyConfigType.Once)
            {
                return dateTime.Date.Add(config.DailyConfiguration.OnceAt.ToTimeSpan());
            }
            // es recurring
            TimeOnly nextExecutionTime = AddOccursEveryUnit(config, dateTimeTime);

            return nextExecutionTime < config.DailyConfiguration.TimeLimits.StartTime
                ? dateTime.Date + config.DailyConfiguration.TimeLimits.StartTime.ToTimeSpan()
                : dateTime.Date + nextExecutionTime.ToTimeSpan();
        }

        private DateTime GetFirstExecutionDate(Configuration config)
        {

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

        private DateTime NextDayExecution(Configuration config, DateTime dateTime)

        {
            if (config.MonthlyConfiguration != null)
            {
                return GetNextMonthDate(config, dateTime);
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
        public DateTime GetNextMonthDate(Configuration config, DateTime dateTime)
        {
            //llega {10/01/2020 3:00:00} 
            var nextMonthDate = dateTime;
            DateTime tempNextMonth;
            if (config.MonthlyConfiguration.Type == MonthlyConfigType.DayNumberOption)
            {
                if (config.DailyConfiguration.Type == DailyConfigType.Once)
                //ONCE
                {
                    return NextDayInMonth(nextMonthDate, config.MonthlyConfiguration);
                }
                else
                //RECURRING
                {//si se ha ejecutado
                 //si esta dentro de los rangos del recurring
                 //si se ha ejecutado devolver nextexecutiontime
                 //si no se  ha ejecutado y esta en el rango,devolver la fecha igual
                    //si no esta dentro de los rangos, devolver el startTime
                    if (Executed)
                    {
                        if (NextExecutionTime(config, nextMonthDate).TimeOfDay <= config.DailyConfiguration.TimeLimits.EndTime.ToTimeSpan())//false
                        {
                            var retornar = new DateTime(nextMonthDate.Year, nextMonthDate.Month, config.MonthlyConfiguration.DayNumber, nextMonthDate.Hour, nextMonthDate.Minute, nextMonthDate.Second);
                            return retornar;
                        }
                        else
                        {
                            return NextDayInMonth(nextMonthDate, config.MonthlyConfiguration);
                        }
                    }
                    else
                    {
                        //tempNextMonth = nextMonthDate.AddMonths(config.MonthlyConfiguration.Frecuency + 1);
                        //nextMonthDate = new DateTime(tempNextMonth.Year, tempNextMonth.Month, 1, 0, 0, 0); //nextmonthdate== {01/05/2020 0:00:00}
                    }
                }
                return NextDayInMonth(nextMonthDate, config.MonthlyConfiguration);
            }
            else
            {
                if (Executed)
                {
                    if (NextExecutionTime(config, nextMonthDate).TimeOfDay <= config.DailyConfiguration.TimeLimits.EndTime.ToTimeSpan())
                    {
                        return nextMonthDate;
                    }
                    else
                    {
                        tempNextMonth = nextMonthDate.AddMonths(config.MonthlyConfiguration.Frecuency);
                        nextMonthDate = new DateTime(tempNextMonth.Year, tempNextMonth.Month, 1, 0, 0, 0);

                        //si salta meses y ha sido ejecutado antes, se va al primer dia del mes hacia el que hizo skip
                        var variablereturn = NextDayOfWeekInMonth(nextMonthDate, config.MonthlyConfiguration);
                        return variablereturn;
                    }
                }
                else
                {
                    return NextDayOfWeekInMonth(nextMonthDate, config.MonthlyConfiguration);
                }
            }
        }
        private static DateTime NextDayOfWeekInMonth(DateTime currentDate, MonthlyConfiguration monthlyConfig)
        {
            Month selectedMonth = new Month(currentDate.Year, currentDate.Month);

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

            IReadOnlyList<DateTime> month;
            if (selectedDays.Count == 1)
            {
                month = selectedMonth.GetMonthDays(selectedDays.First())
                    .Where(x => x >= currentDate)
                    .Select(x => x.Add(TimeOnly.FromDateTime(currentDate).ToTimeSpan()))
                    .ToList();
            }
            else
            {
                //sacar los dias de la semana de la lista selectedDays con linq
                month = selectedMonth.GetMonthDays()
                    .Where(x => x >= currentDate)
                    .Select(x => x.Add(TimeOnly.FromDateTime(currentDate).ToTimeSpan()))
                    .ToList();

                return ObtainOrdinalsFromList(month.Where(x => selectedDays.Contains(x.DayOfWeek)).ToList(), monthlyConfig);
            }
            return ObtainOrdinalsFromList(month, monthlyConfig);
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
        public DateTime NextDayInMonth(DateTime dateTime, MonthlyConfiguration monthlyConfig)
        {
            //solucionar problema al instanciar una fecha que no exista

            int dayNumber = monthlyConfig.DayNumber;
            try
            {
                if (Executed)
                {
                    dateTime = dateTime.AddMonths(monthlyConfig.Frecuency + 1);
                }

                if (dateTime.Day <= dayNumber && dayNumber<= DateTime.DaysInMonth(dateTime.Year, dateTime.Month))
                {
                    //revisar si el dayNumber existe o no en ese mes
                    return new DateTime(dateTime.Year, dateTime.Month, dayNumber,dateTime.Hour,dateTime.Minute,dateTime.Second);
                }
                else
                {
                    //si el dayNumber no existe en ese mes entra aqui
                    //si el numero de dia es 31 y el mes del dateTime no tiene 31, entonces se realiza esto de aqui
                    DateTime nextMonth = dateTime;
                    while (DateTime.DaysInMonth(nextMonth.Year,nextMonth.Month)<dayNumber)
                    {
                        nextMonth = nextMonth.AddMonths(1);
                    }
                    return new DateTime(nextMonth.Year, nextMonth.Month, dayNumber, dateTime.Hour, dateTime.Minute, dateTime.Second);
                }
            }
            catch (ArgumentOutOfRangeException)
            {

                throw new SchedulerException("The selected Monthly day is not valid");
            }
        }
        public static DateTime NextValidDay(DateTime dateTime)
        {

            string dateTimeString = dateTime.ToString("dd/MM/yyyy");


            return dateTime;
        }
        private static TimeOnly AddOccursEveryUnit(Configuration config, TimeOnly dateTimeTime)
        {
            if (config.DailyConfiguration.Frecuency == null)
            {
                return dateTimeTime;
            }

            if (dateTimeTime == TimeOnly.MinValue)
            {
                return config.DailyConfiguration.TimeLimits.StartTime;
            }

            return config.DailyConfiguration.DailyFrecuencyType switch
            {
                DailyFrecuency.Hours => dateTimeTime.AddHours(config.DailyConfiguration.Frecuency.Value),
                DailyFrecuency.Minutes => dateTimeTime.AddMinutes(config.DailyConfiguration.Frecuency.Value),
                DailyFrecuency.Seconds => dateTimeTime.AddSeconds(config.DailyConfiguration.Frecuency.Value),
                _ => dateTimeTime,
            };
        }
    }
}