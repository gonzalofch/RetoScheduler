﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            for (int addedDays=0;dateTime.DayOfWeek!=nextDayOfWeek;dateTime=dateTime.AddDays(1), addedDays++)
            {

            }
            return dateTime.Date;
        }
    }

}
