﻿using Microsoft.Extensions.Localization;
using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using RetoScheduler.Extensions;

namespace RetoScheduler.Runners
{
    public class InRecurringRunner
    {
        private bool Executed = false;

        public InRecurringRunner(bool executed)
        {
            Executed = executed;
        }

        public DateTime Run(Configuration config)
        {
            if (config.DailyConfiguration.Frecuency <= 0)
            {
                throw new SchedulerException("Scheduler:Errors:NegativeDailyFrecuency");
            }

            var firstExecution = GetFirstExecutionDate(config);

            var nextExecution = NextExecutionDate(config, firstExecution);
            //if (nextExecution > nextExecution.Add(config.DailyConfiguration.TimeLimits.EndTime.ToTimeSpan()))
            //{

            //}

            bool hasJumpedDays = config.CurrentDate.Date != nextExecution.Date;

            return NextExecutionTime(config.DailyConfiguration, nextExecution, hasJumpedDays);
        }

        private DateTime NextExecutionTime(DailyConfiguration dailyConfiguration, DateTime dateTime, bool? hasJumpedDays = false)
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

            return DailyRunner.Run(dailyConfiguration, dateTime, Executed);
        }

        private DateTime GetFirstExecutionDate(Configuration config)
        {
            return config.CurrentDate < config.DateLimits.StartDate
                ? config.DateLimits.StartDate
                : config.CurrentDate;
        }

        private DateTime NextExecutionDate(Configuration config, DateTime dateTime)
        {
            bool hasLimits = config.DailyConfiguration.TimeLimits != null;
            var addedHours = DailyRunner.Run(config.DailyConfiguration, dateTime, Executed);
            //var endTimeLimit = config.DailyConfiguration.TimeLimits.EndTime.ToTimeSpan();
            //var endDate = config.DateLimits.EndDate;

            if (config.MonthlyConfiguration != null)
            {
                //si es tipo Daily y recurring(podemos verificar esto) once es directamente monthlyRunner
                //si el siguiente tiempo de ejecucion (addedHours.timeofday > endTime.totimespan()) realizar la ejecucion del monthly
                // si addedHours es menor a endTime, entonces ejecutar m
                DateTime nextDateTime;
                if (config.DailyConfiguration.Type == DailyConfigType.Recurring)
                {
                    if (hasLimits && addedHours.TimeOfDay <= config.DailyConfiguration.TimeLimits.EndTime.ToTimeSpan() )
                    {

                        nextDateTime = MonthlyRunner.Run(config.MonthlyConfiguration, dateTime.JumpToDayNumber(1), Executed);
                    }
                    else
                    {
                        nextDateTime = MonthlyRunner.Run(config.MonthlyConfiguration, dateTime.Date, Executed);
                    }
                }
                else
                {
                    nextDateTime = MonthlyRunner.Run(config.MonthlyConfiguration, dateTime, Executed);
                }

                return nextDateTime.Add(addedHours.TimeOfDay);
            }

            if (config.WeeklyConfiguration == null || !config.WeeklyConfiguration.SelectedDays.Any())
            {
                return dateTime;
            }

            List<DayOfWeek> sortedDays = config.WeeklyConfiguration.SelectedDays.OrderBy(_ => _).ToList();
            DayOfWeek actualDayOfWeek = config.CurrentDate.DayOfWeek;
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
    }
}
