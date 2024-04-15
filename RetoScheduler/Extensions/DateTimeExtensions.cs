using RetoScheduler.Configurations;
using RetoScheduler.Enums;
using RetoScheduler.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RetoScheduler.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime AddWeeks(this DateTime dateTime, int numberOfWeeks)
        {
            return dateTime.AddDays(numberOfWeeks * 7);
        }

        public static DateTime NextDayOfWeek(this DateTime dateTime, DayOfWeek nextDayOfWeek)
        {
            for (int addedDays = 0; dateTime.DayOfWeek != nextDayOfWeek; dateTime = dateTime.AddDays(1), addedDays++)
            {

            }
            return dateTime;
        }
        public static DateTime NextKindOfDay(this DateTime dateTime, Ordinal ordinalPosition, KindOfDay kindOfDay)
        {
            int executionMonth = dateTime.Month; 
            List<KindOfDay> selectedDays = kindOfDay switch
            {
                KindOfDay.WeekDay => new List<KindOfDay>() { KindOfDay.Monday, KindOfDay.Tuesday, KindOfDay.Wednesday, KindOfDay.Thursday, KindOfDay.Friday },
                KindOfDay.WeekEndDay => new List<KindOfDay>() { KindOfDay.Saturday, KindOfDay.Sunday },
                _ => Enum.GetValues(typeof(KindOfDay)).Cast<KindOfDay>().Where(x => x == kindOfDay).ToList(),
            };
            List<DateTime> list = new List<DateTime>();
            while ( dateTime.Month==executionMonth)
            {
                
                if (selectedDays.Contains((KindOfDay)dateTime.DayOfWeek) && kindOfDay != KindOfDay.Day)
                { 
                list.Add(dateTime);
                }
                if (kindOfDay == KindOfDay.Day)
                {
                    list.Add(dateTime);
                }

                dateTime = dateTime.AddDays(1);
            }

            return ordinalPosition switch
            {
                Ordinal.First => list[0],
                Ordinal.Second => list[1],
                Ordinal.Third => list[2],
                Ordinal.Fourth => list[3],
                Ordinal.Last=> list.Last(),
                _ => dateTime,
            };
            
        }
    }
}
