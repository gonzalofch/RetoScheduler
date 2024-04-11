using Microsoft.VisualBasic;
using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using RetoScheduler.Extensions;
using System;
using System.Security.AccessControl;
using System.Security.Authentication.ExtendedProtection;
using System.Text;

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
            if (config.DailyConfiguration.Type == DailyConfigType.Once)
            {
                if ((config.CurrentDate.TimeOfDay > config.DailyConfiguration.OnceAt.ToTimeSpan()))
                {
                    throw new SchedulerException("The execution time cannot be earlier than the Current Time");
                }
            }
            else
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

            string whenOccurs = "";
            string atThisTime;
            if (config.Type == ConfigType.Once)
            {
                whenOccurs += "once";
                string expectedTime = dateTime.ToString("HH:mm");
                atThisTime = " at " + expectedTime;
            }
            else
            {
                if (config.Occurs == Occurs.Weekly)
                {
                    whenOccurs += config.DailyConfiguration.Frecuency == 1
               ? "every week "
               : "every " + config.WeeklyConfiguration.FrecuencyInWeeks + " weeks ";
                    atThisTime = string.Empty;

                    whenOccurs += "on";
                    var selectedDays = config.WeeklyConfiguration.SelectedDays;
                    foreach (var item in selectedDays)
                    {
                        string dayInLower = item.ToString().ToLower();
                        if (item == selectedDays.Last() && selectedDays.Count() >= 2)
                        {
                            whenOccurs += " and " + dayInLower + " ";
                        }
                        else
                        {
                            if (item == selectedDays.First())
                            {
                                whenOccurs += " ";
                            }
                            else
                            {
                                whenOccurs += ", ";
                            }
                            whenOccurs += dayInLower;
                        }
                    }
                }
                //DAILY
                else
                {
                    //    whenOccurs+=config.DailyConfiguration.Frecuency==1
                    //     ? "every day"
                    //: "every " + config. + " days";
                    atThisTime = string.Empty;
                }
            }

            string atThisTimeInDay = string.Empty;
            if (config.DailyConfiguration.Type == DailyConfigType.Once)
            {
                var timeInDay = config.DailyConfiguration.OnceAt;
                atThisTimeInDay = "once at" + timeInDay;
            }
            else
            {
                TimeOnly startTime = config.DailyConfiguration.TimeLimits.StartTime; ;
                string startTimeFormat = (startTime.Hour >= 12)
                ? (startTime.AddHours(-12)).ToString("h:mm tt", System.Globalization.CultureInfo.InvariantCulture)
                : startTime.ToString("h:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                TimeOnly endTime = config.DailyConfiguration.TimeLimits.EndTime; ;
                string endTimeFormat = (endTime.Hour >= 12)
                ? (endTime.AddHours(-12)).ToString("h:mm tt", System.Globalization.CultureInfo.InvariantCulture)  //(endTime.AddHours(-12)).ToString("h:mm tt", System.Globalization.CultureInfo.InvariantCulture) :
                : endTime.ToString("h:mm tt", System.Globalization.CultureInfo.InvariantCulture);

                atThisTimeInDay += config.DailyConfiguration.DailyFrecuencyType == DailyFrecuency.Hours
                    ? "every " + config.DailyConfiguration.Frecuency + " hours "
                    : "every " + config.DailyConfiguration.Frecuency + " minutes ";

                atThisTimeInDay += "between " + startTimeFormat + " and " + endTimeFormat;
            }

            string expectedDate = dateTime.ToString("dd/MM/yyyy");
            string expectedStartLimit = config.DateLimits.StartDate.ToString("dd/MM/yyyy");

            description.Append(whenOccurs)
                .Append(atThisTime)
                .Append(atThisTimeInDay)
                .Append(" starting on ")
                .Append(expectedStartLimit);

            return description.ToString();
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

        private DateTime GetFirstExecutionDate(Configuration config)
        {
            return config.CurrentDate < config.DateLimits.StartDate
               ? config.DateLimits.StartDate
               : config.CurrentDate;
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

            TimeOnly dateTimeTime = TimeOnly.FromDateTime(dateTime);
            if (config.DailyConfiguration.Type == DailyConfigType.Once)
            {
                if (dateTimeTime > config.DailyConfiguration.OnceAt)
                {
                    throw new SchedulerException("Once time execution time can't be before than Current Time");
                }

                return dateTime.Date.Add(dateTimeTime.ToTimeSpan());
            }

            TimeOnly nextExecutionTime = AddOccursEveryUnit(config, dateTimeTime);

            return nextExecutionTime < config.DailyConfiguration.TimeLimits.StartTime
                ? dateTime.Date + config.DailyConfiguration.TimeLimits.StartTime.ToTimeSpan()
                : dateTime.Date + nextExecutionTime.ToTimeSpan();
        }

        private static void ValidateNextExecutionIsBetweenDateLimits(Configuration config, DateTime dateTime)
        {
            var dateBetweenLimits = dateTime >= config.DateLimits.StartDate && (config.DateLimits.EndDate.HasValue == false || dateTime <= config.DateLimits.EndDate);
            if (dateBetweenLimits is false)
            {
                throw new SchedulerException("DateTime can't be out of start and end range");
            }
        }

        private static DateTime NextDayExecution(Configuration config, DateTime dateTime)
        {
            if (config.WeeklyConfiguration == null || !config.WeeklyConfiguration.SelectedDays.Any())
            {
                return dateTime;
            }

            var sortedDays = config.WeeklyConfiguration.SelectedDays.OrderBy(_ => _).ToList();
            var actualDay = config.CurrentDate.DayOfWeek;
            var firstDayOfWeek = config.CurrentDate.TimeOfDay >= config.DailyConfiguration.TimeLimits.EndTime.ToTimeSpan()
            ? sortedDays.FirstOrDefault(_ => _ > actualDay)
            : sortedDays.FirstOrDefault(_ => _ >= actualDay);

            return config.CurrentDate.TimeOfDay >= config.DailyConfiguration.TimeLimits.EndTime.ToTimeSpan()
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

        private static TimeOnly AddOccursEveryUnit(Configuration config, TimeOnly dateTimeTime)
        {
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
