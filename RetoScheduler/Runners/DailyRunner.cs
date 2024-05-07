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
            var date = dateTime.Date;
            TimeSpan addedTime = new(0);

            if (dailyConfiguration.Type == DailyConfigType.Once)
            {
                addedTime = dailyConfiguration.OnceAt.ToTimeSpan();
            }
            else
            {
                var nextExecutionDailyTime = AddOccursEveryUnit(dailyConfiguration, TimeOnly.FromDateTime(dateTime)).ToTimeSpan();
                var startTime = dailyConfiguration.TimeLimits.StartTime.ToTimeSpan();
                addedTime = executed
                    ? GetMinExecutionTimeInDay(nextExecutionDailyTime, startTime)
                    : GetMinExecutionTimeInDay(dateTime.TimeOfDay, startTime);
            //aqui podriamos revisar que date.Add(addedTime) sea menor a endTime, sino excepcion para hacerle saber a la otra clase que debe saltar de dia y luego ejecutar otra vez
            }
            
            return date.Add(addedTime);
        }

        private static TimeSpan GetMinExecutionTimeInDay(TimeSpan time, TimeSpan startTime)
        {
            return time < startTime
                                    ? startTime
                                    : time;
        }

        public static TimeOnly AddOccursEveryUnit(DailyConfiguration dailyConfiguration, TimeOnly time)
        {
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
