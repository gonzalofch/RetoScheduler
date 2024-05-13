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
                if (executed)
                {
                    if (dateTime.TimeOfDay >= onceAtTime)
                    {
                        return onlyDate.Date.AddDays(1).Add(onceAtTime);
                    }
                }

                if (dateTime.TimeOfDay > onceAtTime)
                {
                    return onlyDate.Date.AddDays(1).Add(onceAtTime);
                }
                return onlyDate.Add(onceAtTime);
            }

            var startTime = dailyConfiguration.TimeLimits.StartTime.ToTimeSpan();
            var endTime = dailyConfiguration.TimeLimits.EndTime.ToTimeSpan();

            if (executed && dateTime.TimeOfDay == new TimeSpan(0, 0, 0)) //executed &&
            {

                return onlyDate.Add(startTime);
            }


            if (executed && AddOccursEveryUnit(dailyConfiguration, dateTime).ToTimeSpan() > endTime || dateTime.TimeOfDay > endTime)
            {
                return onlyDate.AddDays(1).Add(startTime);
            }



            return executed
                ? onlyDate.Add(AddOccursEveryUnit(dailyConfiguration, dateTime).ToTimeSpan())
                : GetMinExecutionTime(dateTime, dailyConfiguration);
        }

        private static DateTime GetMinExecutionTime(DateTime dateTime, DailyConfiguration dailyConfiguration)
        {

            var startTime = dailyConfiguration.TimeLimits.StartTime.ToTimeSpan();
            if (dateTime.TimeOfDay > startTime)
            {
                return dateTime;
            }
            if (true)
            {

            }
            return dateTime.Date.Add(startTime);

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
