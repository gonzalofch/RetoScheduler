using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using RetoScheduler.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace RetoScheduler.Runners
{
    public class DailyRunner
    {
        public static DateTime Run(DailyConfiguration dailyConfiguration, DateTime dateTime, bool executed)
        {
            var onlyDate = dateTime.Date;

            if (dailyConfiguration.Type == DailyConfigType.Once)
            {
                var onceAtTime = dailyConfiguration.OnceAt.ToTimeSpan();
                if (dateTime.TimeOfDay>=onceAtTime)
                {
                    return onlyDate.AddDays(1).Add(onceAtTime);

                }
                return onlyDate.Add(onceAtTime);
            }

            var startTime = dailyConfiguration.TimeLimits.StartTime.ToTimeSpan();
            var endTime = dailyConfiguration.TimeLimits.EndTime.ToTimeSpan();

            if (!executed && dateTime.TimeOfDay> endTime)
            {
                onlyDate = dateTime.AddMonths(1).JumpToDayNumber(1).Date;
                return new DateTime (onlyDate.Year, onlyDate.Month, onlyDate.Day,0,0,0);
            }

            if (dateTime.TimeOfDay == new TimeSpan(0, 0, 0)) //executed &&
            {
                return onlyDate.Add(startTime);
            }


            if (executed && AddOccursEveryUnit(dailyConfiguration, dateTime).ToTimeSpan() > endTime || dateTime.TimeOfDay > endTime)
            {
                return onlyDate.AddDays(1).Add(startTime);
            }



            return executed
                ? onlyDate.Add(AddOccursEveryUnit(dailyConfiguration, dateTime).ToTimeSpan())
                : GetMinExecutionTime(dateTime, onlyDate, startTime);
        }

        private static DateTime GetMinExecutionTime(DateTime dateTime, DateTime onlyDate, TimeSpan startTime)
        {
            return dateTime.TimeOfDay <= startTime
                                ? onlyDate.Add(startTime)
                                : dateTime;
        }

        public static TimeOnly AddOccursEveryUnit(DailyConfiguration dailyConfiguration, DateTime dateTime)
        {
            var time = TimeOnly.FromDateTime(dateTime);
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
