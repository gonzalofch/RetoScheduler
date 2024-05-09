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
            if (dailyConfiguration.Type == DailyConfigType.Once)
            {
                if (executed || dateTime.TimeOfDay > dailyConfiguration.OnceAt.ToTimeSpan())
                {
                    return dateTime.Date.AddDays(1).Add(dailyConfiguration.OnceAt.ToTimeSpan());
                }
                return dateTime.Date.Add(dailyConfiguration.OnceAt.ToTimeSpan());
            }

            //recurring
            if (dateTime.TimeOfDay <= dailyConfiguration.TimeLimits.StartTime.ToTimeSpan())
            {
                return dateTime.Date.Add(dailyConfiguration.TimeLimits.StartTime.ToTimeSpan());
            }

            if (dateTime.TimeOfDay > dailyConfiguration.TimeLimits.EndTime.ToTimeSpan())
            {
                return dateTime.Date.AddDays(1).Add(dailyConfiguration.TimeLimits.StartTime.ToTimeSpan());
            }

            if (executed)
            {
                if (AddOccursEveryUnit(dailyConfiguration, dateTime).ToTimeSpan()> dailyConfiguration.TimeLimits.EndTime.ToTimeSpan())
                {
                    return dateTime.Date.AddDays(1).Add(dailyConfiguration.TimeLimits.StartTime.ToTimeSpan());
                }

                return dateTime.Date.Add(AddOccursEveryUnit(dailyConfiguration, dateTime).ToTimeSpan());
            }

            return dateTime;
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
